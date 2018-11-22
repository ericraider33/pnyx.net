using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using pnyx.net.api;
using pnyx.net.errors;
using pnyx.net.impl;
using pnyx.net.impl.bools;
using pnyx.net.impl.columns;
using pnyx.net.impl.csv;
using pnyx.net.impl.sed;
using pnyx.net.processors;
using pnyx.net.processors.readers;
using pnyx.net.shims;
using pnyx.net.util;

namespace pnyx.net.fluent
{
    public class Pnyx : IDisposable
    {
        private readonly ArrayList parts;
        private readonly List<IDisposable> resources;
        private IProcessor processor;
        private IRowConverter rowConverter;
        private FluentState state { get; set; }
        public StreamInformation streamInformation { get; private set; }
            
        public Pnyx()
        {            
            streamInformation = new StreamInformation();
            parts = new ArrayList();
            resources = new List<IDisposable>();
        }

        public Pnyx readStreamFactory(IStreamFactory streamFactory)
        {
            CatModifier cat = retrieveModifier<CatModifier>(stopAtFirstModifier: false);
            if (state == FluentState.Start && cat == null)
                throw new IllegalStateException("Use cat method to read from multiple sources");
            
            if (state != FluentState.New && state != FluentState.Start)
                throw new IllegalStateException("Pnyx is not in New state: {0}", state.ToString());

            parts.Add(new StreamFactoryToLineProcessor(streamInformation, streamFactory));
            state = FluentState.Start;
            return this;
        }

        public Pnyx read(String path)
        {
            return readStreamFactory(new FileStreamFactory(path));
        }

        public Pnyx readStream(Stream input)
        {            
            return readStreamFactory(new GenericStreamFactory(input));
        }

        public Pnyx readString(String source)
        {
            return readStreamFactory(new StringStreamFactory(source));
        }

        public Pnyx cat(Action<Pnyx> pnyxToGroup)
        {
            if (state != FluentState.New)
                throw new IllegalStateException("Pnyx is not in New state: {0}", state.ToString());

            CatModifier cat = new CatModifier();
            int indexToReplace = parts.Count;
            parts.Add(cat);
            state = FluentState.Start;
            
            // Runs actions for grouping
            pnyxToGroup(this);

            if (state == FluentState.New || state == FluentState.Start)
            {
                LineProcessorSequence lpSequence = new LineProcessorSequence();
                for (int i = indexToReplace+1; i < parts.Count;)
                {
                    IProcessor subProcessor = (IProcessor)parts[i];
                    if (!(subProcessor is ILinePart))
                        throw new IllegalStateException("Processor isn't compatible with sequenced line processing: {0}", subProcessor.GetType().Name);
                    
                    parts.RemoveAt(i);
                    lpSequence.processors.Add(subProcessor);
                }
                
                parts[indexToReplace] = lpSequence;            // replace CatModifier with LineSequenceProcessor
                state = FluentState.Line;
            }
            else if (state == FluentState.Row)
            {
                throw new NotImplementedException();
            }

            return this;
        }        
        
        public Pnyx lineToRow(IRowConverter converter)
        {
            if (state == FluentState.Line || state == FluentState.Start)
            {
                parts.Add(new LineToRowProcessor { rowConverter = converter });
                state = FluentState.Row;
                rowConverter = converter;
            }
            else
                throw new IllegalStateException("Pnyx is not in Line, Row, or Start state: {0}", state.ToString());

            return this;
        }

        public Pnyx rowToLine(IRowConverter converter = null)
        {
            if (state == FluentState.Row)
            {
                converter = converter ?? rowConverter;
                parts.Add(new RowToLineProcessor { rowConverter = converter });
                state = FluentState.Line;
                rowConverter = null;
            }
            else
                throw new IllegalStateException("Pnyx is not in Row state: {0}", state.ToString());

            return this;
        }

        public Pnyx print(String format)
        {            
            if (state == FluentState.Row)
            {
                parts.Add(new Print { format = format, rowConverter = rowConverter });
                state = FluentState.Line;
                rowConverter = null;
            }
            else if (state == FluentState.Start || state == FluentState.Line)
            {
                parts.Add(new Print { format = format });
                state = FluentState.Line;                
            }
            else
                throw new IllegalStateException("Pnyx is not in Row,Line or Start state: {0}", state.ToString());

            return this;                
        }

        public Pnyx selectColumns(params int[] columnNumbers)
        {
            int[] indexes = convertColumnNumbersToIndex(columnNumbers);            
            return rowTransformer(new SelectColumns { indexes = indexes });
        }
        
        public Pnyx printColumn(int columnNumber)            // 1-based to be consistent with print and sed
        {
            if (state == FluentState.Row)
            {
                if (columnNumber <= 0)
                    throw new InvalidArgumentException("Invalid ColumnNumber {0}, ColumnNumbers start at 1", columnNumber);
                
                parts.Add(new ColumnToLine { index = columnNumber-1 });
                state = FluentState.Line;
                rowConverter = null;
            }
            else
                throw new IllegalStateException("Pnyx is not in Row state: {0}", state.ToString());

            return this;
        }
                
        public Pnyx withColumns(Action<Pnyx> block, params int[] columnNumbers)
        {
            if (state != FluentState.Row)
                throw new IllegalStateException("Pnyx is not in Row state: {0}", state.ToString());
               
            // Add modifier to parts
            int partIndex = parts.Count;
            int[] indexes = convertColumnNumbersToIndex(columnNumbers);
            parts.Add(new WithColumns { indexes = indexes });
            
            // Runs block
            block(this);
            
            // Removes modifier from parts
            parts.RemoveAt(partIndex);

            return this;
        }

        private int[] convertColumnNumbersToIndex(int[] columnNumbers)
        {
            if (columnNumbers.Length == 0)
                throw new InvalidArgumentException("Must specify at least one ColumnNumber");
                
            int[] indexes = new int[columnNumbers.Length];
            for (int i = 0; i < columnNumbers.Length; i++)
            {
                int columnNumber = columnNumbers[i];
                if (columnNumber <= 0)
                    throw new InvalidArgumentException("Invalid ColumnNumber {0}, ColumnNumbers start at 1", columnNumber);

                indexes[i] = columnNumber - 1;
            }

            return indexes;
        }
                
        public Pnyx parseCsv(bool strict = true)
        {
            ILineSource lineSource = (parts.Count == 0 ? null : parts[parts.Count-1]) as ILineSource;
            if (state == FluentState.Start && lineSource != null)
            {
                CsvStreamToRowProcessor csv = new CsvStreamToRowProcessor();
                csv.setStrict(strict);
                csv.setSource(streamInformation, lineSource.streamFactory);
                rowConverter = csv.getRowConverter();

                parts[parts.Count - 1] = csv;
                state = FluentState.Row;                
            }
            else if (state == FluentState.Line)
            {
                CsvRowConverter rc = new CsvRowConverter();
                rc.setStrict(strict);
                lineToRow(rc);
            }
            else 
                throw new IllegalStateException("Pnyx is not in Start or Line state: {0}", state.ToString());

            return this;
        }

        public Pnyx parseDelimiter(String delimiter)
        {
            return lineToRow(new DelimiterRowConverter { delimiter = delimiter });
        }

        public Pnyx parseTab()
        {
            return lineToRow(new DelimiterRowConverter { delimiter = "\t" });
        }

        public Pnyx printDelimiter(String delimiter)
        {
            return rowToLine(new DelimiterRowConverter { delimiter = delimiter });
        }

        public Pnyx printTab()
        {
            return rowToLine(new DelimiterRowConverter { delimiter = "\t" });
        }

        public Pnyx linePart(ILinePart linePart)
        {
            if (state == FluentState.Line || state == FluentState.Start)
            {
                parts.Add(linePart);
                state = FluentState.Line;
                return this;
            }
            else if (state == FluentState.Row)
            {
                // look for SHIM
                
                // else convert to LINE
                
                throw new NotImplementedException("Code ROW to LINE");
                
            }
            else
                throw new IllegalStateException("Pnyx is not in Line, Row, or Start state: {0}", state.ToString());
        }

        public Pnyx lineFilter(ILineFilter filter)
        {
            // Process any modifier
            ILineFilterModifier modifier = retrieveModifier<ILineFilterModifier>();
            if (modifier != null)
                filter = modifier.modifyLineFilter(filter);                        // wraps filter according to active modifier
            
            if (state == FluentState.Row)
            {
                if (filter is IRowFilter)
                    return rowFilter((IRowFilter) filter);
                else
                    return rowFilter(new RowFilterShim { lineFilter = filter });                
            }
                
            return linePart(new LineFilterProcessor { filter = filter });
        }
        
        public Pnyx lineTransformer(ILineTransformer transform)
        {
            // Process any modifier
            ILineTransformerModifier modifier = retrieveModifier<ILineTransformerModifier>();
            if (modifier != null)
                transform = modifier.modifyLineTransformer(transform);                        // wraps filter according to active modifier            
            
            if (state == FluentState.Row)
            {
                if (transform is IRowTransformer)
                    return rowTransformer((IRowTransformer)transform);
                else
                    return rowTransformer(new RowTransformerShim { lineTransformer = transform });                
            }

            return linePart(new LineTransformerProcessor { transform = transform });
        }             
        
        public Pnyx lineBuffering(ILineBuffering transform)
        {
            if (state == FluentState.Row)
            {
                if (transform is IRowBuffering)
                    return rowBuffering((IRowBuffering) transform);
                else                        
                    throw new IllegalStateException("Convert to Line before using lineBuffering");                
            }
            
            return linePart(new LineBufferingProcessor { transform = transform });
        }             
        
        public Pnyx lineFilter(Func<String, bool> filter)
        {            
            if (state == FluentState.Row)
                return rowFilter(new RowFilterFuncShim { lineFilter = filter });

            return linePart(new LineFilterFuncProcessor { transform = filter });
        }
        
        public Pnyx lineTransformer(Func<String, String> transform)
        {
            if (state == FluentState.Row)
                return rowTransformer(new RowTransformerFuncShim { lineTransformer = transform });                

            return linePart(new LineTransformerFuncProcessor { transform = transform });
        }           
        
        public Pnyx rowPart(IRowPart rowPart)
        {
            if (state == FluentState.Line)
            {
                if (rowConverter == null)
                    throw new IllegalStateException("Specify a RowConverter before adding Row parts");
                
                throw new NotImplementedException("Code LINE to ROW");
            }
            else if (state == FluentState.Row || state == FluentState.Start)
            {
                parts.Add(rowPart);
                state = FluentState.Row;
                return this;
            }
            else
                throw new IllegalStateException("Pnyx is not in Line, Row, or Start state: {0}", state.ToString());
        }

        public Pnyx rowFilter(IRowFilter rowFilter)
        {
            // Process any modifier
            IRowFilterModifier modifier = retrieveModifier<IRowFilterModifier>();
            if (modifier != null)
                rowFilter = modifier.modifyRowFilter(rowFilter);                        // wraps filter according to active modifier
                        
            return rowPart(new RowFilterProcessor { filter = rowFilter });
        }

        public Pnyx rowTransformer(IRowTransformer transform)
        {
            // Process any modifier
            IRowTransformerModifer modifier = retrieveModifier<IRowTransformerModifer>();
            if (modifier != null)
                transform = modifier.modifyRowTransformer(transform);                  // wraps filter according to active modifier
            
            return rowPart(new RowTransformerProcessor { transform = transform });
        }

        public Pnyx rowBuffering(IRowBuffering transform)
        {
            return rowPart(new RowBufferingProcessor { transform = transform });
        }

        public Pnyx grep(String textToFind, bool caseSensitive = true, bool invert = false)
        {
            return lineFilter(new Grep { textToFind = textToFind, caseSensitive = caseSensitive, invert = invert });
        }

        public Pnyx sed(String pattern, String replacement, String flags = null)
        {
            return lineTransformer(new SedReplace(pattern, replacement, flags));
        }

        public Pnyx sedLineNumber()
        {
            return lineBuffering(new SedLineNumber());
        }

        public Pnyx sedAppendRow(String[] toAppend)
        {
            return rowBuffering(new SedAppendRow { text = toAppend });
        }

        public Pnyx sedAppendLine(String text)
        {
            return lineBuffering(new SedAppendLine { text = text });            
        }

        public Pnyx sedInsert(String text)
        {
            return lineBuffering(new SedInsert { text = text });
        }

        private Pnyx setEnd(Stream output, Object destination = null)
        {
            if (state != FluentState.Line && state != FluentState.Row && state != FluentState.Start)
                throw new IllegalStateException("Pnyx is not in Line, Row, or Start state: {0}", state.ToString());

            IRowConverter endRowConverter = destination as IRowConverter;
            ILineProcessor lineDestination = destination as ILineProcessor;
            
            if ((state == FluentState.Start || state == FluentState.Line) && endRowConverter != null)
                lineToRow(endRowConverter);                

            if (state == FluentState.Row && lineDestination != null)
            {
                //TODO insert a rowToLine conversion and then compile as LINE
                throw new NotImplementedException("TODO");    
            }
            
            if (state == FluentState.Row)
            {
                endRowConverter = endRowConverter ?? rowConverter;
                IRowProcessor rowDestination = endRowConverter.buildRowDestination(streamInformation, output);

                if (rowDestination == null)
                    rowToLine();                    // converts to line, then falls through to (state == line)
                else
                {
                    parts.Add(rowDestination);
                    state = FluentState.End;
                }
            }

            if (state == FluentState.Line || state == FluentState.Start)
            {
                lineDestination = lineDestination ?? new LineProcessorToStream(streamInformation, output);                
                parts.Add(lineDestination);
                state = FluentState.End;
            }
            
            return this;
        }

        public Pnyx compile()
        {
            foreach (Object part in parts)
                if (part is IDisposable)
                    resources.Add((IDisposable)part);
            
            Object last = parts[parts.Count - 1]; 
            for (int i = parts.Count-2; i >= 0; i--)
            {
                Object part = parts[i];

                if (part is IRowPart)
                {
                    IRowPart currentPart = (IRowPart)part;
                    currentPart.setNext((IRowProcessor)last);
                }
                else if (part is ILinePart)
                {
                    ILinePart currentPart = (ILinePart)part;
                    currentPart.setNext((ILineProcessor)last);
                }
                else
                    throw new IllegalStateException("Unknown part {0} should be consumed before compiling", part.GetType().Name);

                last = part;                    
            }

            processor = (IProcessor) parts[0];
            
            state = FluentState.Compiled;
            return this;
        }        

        public Pnyx write(String path)
        {
            return setEnd(new FileStream(path, FileMode.Create, FileAccess.Write));
        }

        public Pnyx writeStream(Stream output)
        {
            return setEnd(output);
        }

        public Pnyx writeCsv(String path, bool strict = true)
        {
            return setEnd(new FileStream(path, FileMode.Create, FileAccess.Write), new CsvRowConverter().setStrict(strict));            
        }

        public Pnyx writeCsv(Stream stream, bool strict = true)
        {
            return setEnd(stream, new CsvRowConverter().setStrict(strict));            
        }

        public String processToString(Action<Pnyx,Stream> writeAction = null)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                if (writeAction != null)
                    writeAction(this, stream);
                else
                    writeStream(stream);
                
                process();

                return streamInformation.encoding.GetString(stream.ToArray());
            }
        }

        public Pnyx process()
        {
            if (state == FluentState.End)
                compile();
            
            if (state != FluentState.Compiled)
                throw new IllegalStateException("Pnyx must have an end point before processing");
                                        
            processor.process();
            state = FluentState.Processed;

            return this;
        }

        public void Dispose()
        {
            if (state == FluentState.Disposed)
                return;
            
            foreach (IDisposable resource in resources)
                resource.Dispose();                                    
            resources.Clear();

            state = FluentState.Disposed;
        }

        private ModifierType retrieveModifier<ModifierType>(bool stopAtFirstModifier = true) where ModifierType : class, IModifier 
        {
            // finds first modifier, scanning from end of parts
            for (int i = parts.Count - 1; i >= 0; i--)
            {
                IModifier modifier = parts[i] as IModifier;
                if (modifier == null)
                    continue;

                ModifierType result = modifier as ModifierType;
                if (stopAtFirstModifier || result != null)                
                    return result;
            }

            return null;
        }

        // groups 0,1, or more filters.  Allows 0 and 1 so that any variable number of filters are treated as 1
        private Pnyx groupFilters(Action<Pnyx> pnyxToGroup,
            Func<IEnumerable<ILineFilter>, ILineFilter> lineFilterFactory,
            Func<IEnumerable<IRowFilter>, IRowFilter> rowFilterFactory            
            )
        {
            FluentState current = state;
            
            if (state != FluentState.Line && state != FluentState.Row && state != FluentState.Start)
                throw new IllegalStateException("Pnyx is not in Line, Row, or Start state: {0}", state.ToString());

            int before = parts.Count;
            pnyxToGroup(this);
            
            if (current != FluentState.Start && current != state)
                throw new IllegalStateException("State changed to {0} during groupFilters operation, which is not permitted", state.ToString());

            if (state == FluentState.Row)
            {
                List<IRowFilter> filters = new List<IRowFilter>();
                int i = before;
                while (i < parts.Count)
                {
                    RowFilterProcessor part = parts[i] as RowFilterProcessor;                
                    if (part == null)
                        throw new IllegalStateException("groupFilters only supports filters, but found processor of {0}", parts[i].GetType().Name);
                
                    parts.RemoveAt(i);
                    filters.Add(part.filter);
                }
                
                return rowFilter(rowFilterFactory(filters));                
            }
            else
            {
                List<ILineFilter> filters = new List<ILineFilter>();
                int i = before;
                while (i < parts.Count)
                {
                    LineFilterProcessor part = parts[i] as LineFilterProcessor;                
                    if (part == null)
                        throw new IllegalStateException("groupFilters only supports filters, but found processor of {0}", parts[i].GetType().Name);
                
                    parts.RemoveAt(i);
                    filters.Add(part.filter);
                }

                return lineFilter(lineFilterFactory(filters));
            }            
        }

        public Pnyx and(Action<Pnyx> pnyxToGroup)
        {
            return groupFilters(pnyxToGroup,
                x => new AndLineFilter(x),
                x => new AndRowFilter(x)
            );
        }

        public Pnyx or(Action<Pnyx> pnyxToGroup)
        {
            return groupFilters(pnyxToGroup,
                x => new OrLineFilter(x),
                x => new OrRowFilter(x)
            );
        }

        public Pnyx xor(Action<Pnyx> pnyxToGroup)
        {
            return groupFilters(pnyxToGroup,
                x => new XorLineFilter(x),
                x => new XorRowFilter(x)
            );
        }

        public Pnyx not(Action<Pnyx> pnyxToGroup)
        {
            return groupFilters(pnyxToGroup,
                x => new NotLineFilter(x),
                x => new NotRowFilter(x)
            );
        }

        public Pnyx beforeAfterFilter(int before, int after, Action<Pnyx> pnyxToGroup)
        {
            and(pnyxToGroup);

            Object partRaw = parts[parts.Count - 1];
            parts.RemoveAt(parts.Count - 1);
            
            if (state == FluentState.Row)
            {
                RowFilterProcessor part = partRaw as RowFilterProcessor;                
                if (part == null)
                    throw new IllegalStateException("groupFilters only supports filters, but found processor of {0}", partRaw.GetType().Name);

                return rowBuffering(new BeforeAfterRowBuffering(before, after, part.filter));
            }
            else
            {
                LineFilterProcessor part = partRaw as LineFilterProcessor;                
                if (part == null)
                    throw new IllegalStateException("groupFilters only supports filters, but found processor of {0}", partRaw.GetType().Name);
                
                return lineBuffering(new BeforeAfterLineBuffering(before, after, part.filter));
            }                       
        }
    }
}