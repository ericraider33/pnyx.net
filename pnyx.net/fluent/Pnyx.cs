using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using pnyx.net.api;
using pnyx.net.errors;
using pnyx.net.impl;
using pnyx.net.impl.columns;
using pnyx.net.impl.groups;
using pnyx.net.impl.sed;
using pnyx.net.processors;
using pnyx.net.shims;
using pnyx.net.util;

namespace pnyx.net.fluent
{
    public class Pnyx : IDisposable
    {
        private Stream start;
        private readonly ArrayList parts;
        private readonly List<IDisposable> resources;
        private IProcessor processor;
        private IRowSource rowSource;
        private IRowConverter rowConverter;
        private FluentState state { get; set; }
        public StreamInformation streamInformation { get; private set; }
            
        public Pnyx()
        {            
            streamInformation = new StreamInformation();
            parts = new ArrayList();
            resources = new List<IDisposable>();
        }

        private void setStart(Stream start)
        {
            if (state != FluentState.New)
                throw new IllegalStateException("Pnyx is not in New state: {0}", state.ToString());

            this.start = start;
            state = FluentState.Start;
        }

        public Pnyx read(String path)
        {
            setStart(new FileStream(path, FileMode.Open, FileAccess.Read));
            return this;
        }

        public Pnyx readStream(Stream input)
        {
            setStart(input);
            return this;
        }

        public Pnyx readString(String source)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);

            writer.Write(source);
            writer.Flush();
            stream.Position = 0;

            setStart(stream);
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
            if (state == FluentState.Start)
            {
                CsvStreamToRowProcessor csv = new CsvStreamToRowProcessor();
                csv.setStrict(strict);
                rowSource = csv;
                rowConverter = csv.getRowConverter();
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
            ILineFilterModifier modifier = retrieveTopModifier<ILineFilterModifier>();
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
            ILineTransformerModifier modifier = retrieveTopModifier<ILineTransformerModifier>();
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
                    throw new NotImplementedException();                
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
                    throw new IllegalStateException("Specify a RowConverter to before adding Row parts");
                
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
            IRowFilterModifier modifier = retrieveTopModifier<IRowFilterModifier>();
            if (modifier != null)
                rowFilter = modifier.modifyRowFilter(rowFilter);                        // wraps filter according to active modifier
                        
            return rowPart(new RowFilterProcessor { filter = rowFilter });
        }

        public Pnyx rowTransformer(IRowTransformer transform)
        {
            // Process any modifier
            IRowTransformerModifer modifier = retrieveTopModifier<IRowTransformerModifer>();
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

        public Pnyx sedAppend(String text)
        {
            return lineBuffering(new SedAppend { text = text });
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
            
            resources.Add(output);        // marks for cleanup            
            return this;
        }

        public Pnyx compile()
        {
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
                        
            if (rowSource != null)
            {
                rowSource.setSource(streamInformation, start, (IRowProcessor)last);
                processor = rowSource;
            }
            else if (last is ILineProcessor)
            {
                processor = new StreamToLineProcessor(streamInformation, start, (ILineProcessor)last);
            }
            else
                throw new IllegalStateException("Unknown part {0} should be consumed before compiling", last.GetType().Name);                        
            
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
            
            if (start != null)
                start.Dispose();
            start = null;

            state = FluentState.Disposed;
        }

        private ModifierType retrieveTopModifier<ModifierType>() where ModifierType : class, IModifier 
        {
            // finds first modifier, scanning from end of parts
            for (int i = parts.Count - 1; i >= 0; i--)
            {
                IModifier modifier = parts[i] as IModifier;
                if (modifier == null)
                    continue;

                ModifierType result = modifier as ModifierType;
                return result;
            }

            return null;
        }

        // groups 0,1, or more filters.  Allows 0 and 1 so that any variable number of filters are treated as 1
        public Pnyx groupFilters(Action<Pnyx> pnyxToGroup)
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
                RowFilterGroup group = new RowFilterGroup();
                int i = before;
                while (i < parts.Count)
                {
                    RowFilterProcessor part = parts[i] as RowFilterProcessor;                
                    if (part == null)
                        throw new IllegalStateException("groupFilters only supports filters, but found processor of {0}", part.GetType().Name);
                
                    parts.RemoveAt(i);
                    group.filters.Add(part.filter);
                }

                return rowFilter(group);                
            }
            else
            {
                LineFilterGroup group = new LineFilterGroup();
                int i = before;
                while (i < parts.Count)
                {
                    LineFilterProcessor part = parts[i] as LineFilterProcessor;                
                    if (part == null)
                        throw new IllegalStateException("groupFilters only supports filters, but found processor of {0}", part.GetType().Name);
                
                    parts.RemoveAt(i);
                    group.filters.Add(part.filter);
                }

                return lineFilter(group);
            }            
        }

        public Pnyx beforeAfterFilter(int before, int after, Action<Pnyx> pnyxToGroup)
        {
            groupFilters(pnyxToGroup);

            Object partRaw = parts[parts.Count - 1];
            parts.RemoveAt(parts.Count - 1);
            
            if (state == FluentState.Row)
            {
                RowFilterProcessor part = partRaw as RowFilterProcessor;                
                if (part == null)
                    throw new IllegalStateException("groupFilters only supports filters, but found processor of {0}", part.GetType().Name);

//                return rowBuffering(new BeforeAfterBuffering(before, after, null, processor.filter));
                throw new NotImplementedException("Code ROW version");
            }
            else
            {
                LineFilterProcessor part = partRaw as LineFilterProcessor;                
                if (partRaw == null)
                    throw new IllegalStateException("groupFilters only supports filters, but found processor of {0}", part.GetType().Name);
                
                return lineBuffering(new BeforeAfterBuffering(before, after, part.filter, null));
            }                       
        }
    }
}