using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using pnyx.net.api;
using pnyx.net.errors;
using pnyx.net.impl;
using pnyx.net.impl.bools;
using pnyx.net.impl.columns;
using pnyx.net.impl.csv;
using pnyx.net.impl.sed;
using pnyx.net.processors;
using pnyx.net.processors.converters;
using pnyx.net.processors.dest;
using pnyx.net.processors.lines;
using pnyx.net.processors.rows;
using pnyx.net.processors.sort;
using pnyx.net.processors.sources;
using pnyx.net.shims;
using pnyx.net.util;

namespace pnyx.net.fluent
{
    public class Pnyx : IDisposable
    {
        public FluentState state { get; private set; }
        public StreamInformation streamInformation { get; private set; }
        public readonly Settings settings;
        private IProcessor processor;
        private IRowConverter rowConverter;
        private readonly ArrayList parts;
        private readonly List<IDisposable> resources;
        private readonly List<String> sourceFiles;
        
        private EventHandler<Pnyx> stateProcessed;
        public event EventHandler<Pnyx> StateProcessed
        {
            add => stateProcessed += value;
            remove => stateProcessed -= value;
        }
            
        public Pnyx(Settings settings = null, StreamInformation streamInformation = null)
        {
            this.settings = settings ?? SettingsHome.settingsFactory.buildSettings();
            this.streamInformation = streamInformation ?? this.settings.buildStreamInformation();
            
            parts = new ArrayList();
            resources = new List<IDisposable>();
            sourceFiles = new List<String>();
        }

        public Pnyx setSettings(
            String tempDirectory = null,
            int? bufferLines = null,
            Encoding defaultEncoding = null,
            String defaultNewline = null,
            bool? backupRewrite = null,
            bool? processOnDispose = null
            )
        {
            if (tempDirectory != null) settings.tempDirectory = tempDirectory;
            if (bufferLines != null) settings.bufferLines = bufferLines.Value;
            if (defaultEncoding != null) { settings.defaultEncoding = defaultEncoding; streamInformation.defaultEncoding = defaultEncoding; }
            if (defaultNewline != null) { settings.defaultNewline = defaultNewline; streamInformation.defaultNewline = defaultNewline; }
            if (backupRewrite != null) settings.backupRewrite = backupRewrite.Value;
            if (processOnDispose != null) settings.processOnDispose = processOnDispose.Value;

            return this;
        }

        public Pnyx startLine(ILinePart lineProcessor)
        {
            if (state != FluentState.New || parts.Count > 0)
                throw new IllegalStateException("Pnyx is not in New state: {0}", state.ToString());
            
            parts.Add(lineProcessor);
            state = FluentState.Line;
            
            return this;
        }

        public Pnyx startRow(IRowPart rowProcessor, IRowConverter rowConverter)
        {
            if (state != FluentState.New || parts.Count > 0)
                throw new IllegalStateException("Pnyx is not in New state: {0}", state.ToString());
            
            parts.Add(rowProcessor);
            this.rowConverter = rowConverter;
            state = FluentState.Row;
            
            return this;            
        }

        public Pnyx readStreamFactory(IStreamFactory streamFactory)
        {
            CatModifier cat = retrieveModifier<CatModifier>(stopAtFirstModifier: false);
            if (state == FluentState.Start && cat == null)
                throw new IllegalStateException("Use cat method to read from multiple sources");
            
            if (state != FluentState.New && state != FluentState.Start)
                throw new IllegalStateException("Pnyx is not in New state: {0}", state.ToString());

            // Check for wrapper
            IStreamFactoryWrapper wrapper = retrieveModifier<IStreamFactoryWrapper>(consume: true);
            if (wrapper != null)
                streamFactory = wrapper.wrapStreamFactory(streamFactory);
            
            // Builds processor
            IStreamFactoryModifier streamModifier = retrieveModifier<IStreamFactoryModifier>();
            IProcessor readProcessor;
            if (streamModifier != null)
                readProcessor = streamModifier.buildProcessor(streamInformation, streamFactory);
            else
                readProcessor = new StreamToLineProcessor(streamInformation, streamFactory);

            // Check if processor is a row-source
            IRowSource rowSource = readProcessor as IRowSource;
            if (rowSource != null)
                rowConverter = rowSource.getRowConverter();
            
            parts.Add(readProcessor);
            state = FluentState.Start;
            return this;
        }

        public Pnyx read(String path)
        {
            sourceFiles.Add(path);
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

        public Pnyx readStdin()
        {
            return readStream(Console.OpenStandardInput());
        }

        public Pnyx cat(Action<Pnyx> block)
        {
            if (state != FluentState.New)
                throw new IllegalStateException("Pnyx is not in New state: {0}", state.ToString());

            CatModifier cat = new CatModifier();
            int indexToReplace = parts.Count;
            parts.Add(cat);
            state = FluentState.Start;
            
            // Runs actions for grouping
            block(this);

            if (state == FluentState.New || state == FluentState.Start)
            {
                LineProcessorSequence lpSequence = new LineProcessorSequence(streamInformation);
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
                RowProcessorSequence rpSequence = new RowProcessorSequence(streamInformation);
                for (int i = indexToReplace+1; i < parts.Count;)
                {
                    IProcessor subProcessor = (IProcessor)parts[i];
                    if (!(subProcessor is IRowPart))
                        throw new IllegalStateException("Processor isn't compatible with sequenced row processing: {0}", subProcessor.GetType().Name);
                    
                    parts.RemoveAt(i);
                    rpSequence.processors.Add(subProcessor);
                }
                
                parts[indexToReplace] = rpSequence;            // replace CatModifier with LineSequenceProcessor
            }

            return this;
        }

        public Pnyx asCsv(Action<Pnyx> block, bool strict = true, bool hasHeader = false)
        {
            if (state != FluentState.New && state != FluentState.Start)
                throw new IllegalStateException("Pnyx is not in New,Start state: {0}", state.ToString());

            int indexModifier = parts.Count;
            parts.Add(new CsvModifer { strict = strict, hasHeader = hasHeader });

            block(this);

            if (state != FluentState.Start)
                throw new IllegalStateException("CSV modifier only accepts allows reads: {0}", state.ToString());

            state = FluentState.Row;
            parts.RemoveAt(indexModifier);
            
            return this;
        }

        public Pnyx head(int limit = 1)
        {
            if (limit < 1)
                throw new InvalidArgumentException("Head limit must be greater than zero");

            if (state == FluentState.Start || state == FluentState.Line)
                return lineFilter(new HeadFilter(streamInformation, limit));
            else if (state == FluentState.Row)
                return rowFilter(new HeadFilter(streamInformation, limit));
            else
                throw new IllegalStateException("Pnyx is not in Line,Row,Start state: {0}", state.ToString());
        }

        public Pnyx tail(int limit = 1)
        {
            if (limit < 1)
                throw new InvalidArgumentException("Head limit must be greater than zero");

            if (state != FluentState.New && state != FluentState.Start)
                throw new IllegalStateException("Pnyx is not in New,Start state: {0}", state.ToString());

            parts.Add(new TailModifier(limit, streamInformation));
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

        public Pnyx columnDefinition(int? limit = null, bool maxWidth = false, bool hasHeaderRow = false, bool minWidth = false, bool nullable = false)
        {
            ColumnDefinition buffering = new ColumnDefinition(streamInformation);
            if (limit.HasValue)
                buffering.limit = limit.Value;

            ColumnDefinition.Flags flag = ColumnDefinition.Flags.None;
            if (hasHeaderRow) flag |= ColumnDefinition.Flags.Header;
            if (maxWidth) flag |= ColumnDefinition.Flags.MaxWidth;
            if (minWidth) flag |= ColumnDefinition.Flags.MinWidth;
            if (nullable) flag |= ColumnDefinition.Flags.Nullable;
            buffering.flag = flag;

            return rowBuffering(buffering);
        }

        public Pnyx swapColumnsAndRows()
        {
            return rowBuffering(new SwapColumnsAndRows());
        }

        public Pnyx hasColumns(bool verifyColumnHasText, params int[] columnNumbers)
        {
            return rowFilter(new HasColumns(columnNumbers, verifyColumnHasText));
        }

        public Pnyx widthColumns(int columns, String pad = "")
        {
            return rowTransformer(new WidthColumns { columns = columns, pad = pad });
        }

        public Pnyx removeColumns(params int[] columnNumbers)
        {            
            return rowTransformer(new RemoveColumns(columnNumbers));
        }

        public Pnyx insertColumnsWithPadding(String pad = "", params int[] columnNumbers)
        {
            return rowTransformer(new InsertColumns(columnNumbers) { pad = pad });
        }

        public Pnyx insertColumns(params int[] columnNumbers)
        {
            return rowTransformer(new InsertColumns(columnNumbers));
        }

        public Pnyx duplicateColumnsWithPadding(String pad = "", params int[] columnNumbers)
        {
            return rowTransformer(new DuplicateColumns(columnNumbers) { pad = pad });
        }

        public Pnyx duplicateColumns(params int[] columnNumbers)
        {
            return rowTransformer(new DuplicateColumns(columnNumbers));
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
                
        public Pnyx parseCsv(bool strict = true, bool hasHeader = false)
        {
            ILineSource lineSource = (parts.Count == 0 ? null : parts[parts.Count-1]) as ILineSource;
            if (state == FluentState.Start && lineSource != null)
            {
                CsvStreamToRowProcessor csv = new CsvStreamToRowProcessor();
                csv.hasHeader = hasHeader;
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
            else
                throw new IllegalStateException("Pnyx is not in Line, or Start state: {0}", state.ToString());
        }

        public Pnyx lineFilter(ILineFilter filter)
        {
            // Process any modifier
            ILineFilterModifier modifier = retrieveModifier<ILineFilterModifier>();
            if (modifier != null)
                filter = modifier.modifyLineFilter(filter);                        // wraps filter according to active modifier
            
            if (state == FluentState.Row)
            {
                IRowFilterShimModifier shimModifier = retrieveModifier<IRowFilterShimModifier>(); 
                if (filter is IRowFilter)
                    return rowFilter((IRowFilter) filter);
                else if (shimModifier != null)
                    return rowFilter(shimModifier.shimLineFilter(filter));                
                else
                    return rowFilter(new RowFilterShimOr { lineFilter = filter });                
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
                IRowTransformerShimModifier shimModifier = retrieveModifier<IRowTransformerShimModifier>();                
                if (transform is IRowTransformer)
                    return rowTransformer((IRowTransformer)transform);
                else if (shimModifier != null)
                    return rowTransformer(shimModifier.shimLineTransformer(transform));
                else
                    return rowTransformer(new RowTransformerShimOr { lineTransformer = transform });                
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
        
        public Pnyx lineFilterFunc(Func<String, bool> filter)
        {
            return lineFilter(new LineFilterFunc { lineFilterFunc = filter });
        }
        
        public Pnyx lineTransformerFunc(Func<String, String> transform)
        {
            return lineTransformer(new LineTransformerFunc { lineTransformerFunc = transform });
        }

        public Pnyx shimAnd(Action<Pnyx> block)
        {
            if (state != FluentState.Row)
                throw new IllegalStateException("Shim is only needed in Row state: {0}", state.ToString());

            int toRemove = parts.Count;
            parts.Add(new AndShimModifier());

            // Runs wrapped
            block(this);
            
            if (state != FluentState.Row)
                throw new IllegalStateException("Shim is only supports Row actions: {0}", state.ToString());

            parts.RemoveAt(toRemove);
            return this;
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
        
        public Pnyx rowFilterFunc(Func<String[], bool> filter)
        {
            return rowFilter(new RowFilterFunc { rowFilterFunc = filter });
        }
        
        public Pnyx rowTransformerFunc(Func<String[], String[]> transform, bool treatHeaderAsRow = false)
        {
            return rowTransformer(new RowTransformerFunc { rowTransformerFunc = transform, treatHeaderAsRow = treatHeaderAsRow });
        }           
        
        public Pnyx rowBuffering(IRowBuffering transform)
        {
            return rowPart(new RowBufferingProcessor { buffering = transform });
        }

        public Pnyx grep(String textToFind, bool caseSensitive = true)
        {
            return lineFilter(new Grep { textToFind = textToFind, caseSensitive = caseSensitive });
        }

        public Pnyx egrep(String expression, bool caseSensitive = true)
        {
            return lineFilter(new EGrep(expression, caseSensitive));
        }

        public Pnyx hasLine()
        {
            return lineFilter(new HasLine());
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

        public Pnyx compile()
        {
            if (state == FluentState.Compiled || state == FluentState.CompiledServile)
                return this;
            
            if (state != FluentState.End)
                throw new IllegalStateException("Pnyx is not in End state: {0}", state.ToString());
            
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

            processor = parts[0] as IProcessor;

            if (processor == null)
                state = FluentState.CompiledServile;         // first part is dependent upon another source (like a Tee from another Pnyx)
            else
                state = FluentState.Compiled;
            
            return this;
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

        public Pnyx writeCsvStream(Stream stream, bool strict = true)
        {
            return setEnd(stream, new CsvRowConverter().setStrict(strict));            
        }

        public Pnyx writeStdout()
        {
            return setEnd(Console.OpenStandardOutput());
        }

        public Pnyx writeSplit(String fileNamePattern, int limit, String path = null)
        {
            if (!fileNamePattern.Contains("$0"))
                throw new InvalidArgumentException("FileName pattern requires $0 substitution variable");
            
            if (state == FluentState.Row)
                rowToLine();

            return setEnd(null, new LineProcessorSplit(streamInformation, fileNamePattern, limit, path));
        }

        public Pnyx rewrite(bool? backupOriginal = null, bool? deleteBackup = null)
        {
            bool backupOriginal_ = backupOriginal ?? settings.backupRewrite;
            bool deleteBackup_ = deleteBackup ?? !settings.backupRewrite;
            
            if (sourceFiles.Count != 1)
                throw new InvalidArgumentException("rewrite is only valid when source is a single file, but found source files of: {0}", sourceFiles.Count);
            
            String tempFile = Path.Combine(settings.tempDirectory, Guid.NewGuid() + ".tmp");

            String sourcePath = sourceFiles[0];

            String sourceFileName = Path.GetFileName(sourcePath);
            String backupFile = Path.Combine(settings.tempDirectory, sourceFileName + Guid.NewGuid());

            // Adds hook to move file after processing is complete
            stateProcessed += (sender, pnyx) =>
            {
                // Backs up original file
                if (backupOriginal_)
                    File.Move(sourcePath, backupFile);
                else
                    File.Delete(sourcePath);
                                
                // Rewrites output to original source path
                File.Move(tempFile, sourcePath);
                
                // Deletes back up
                if (backupOriginal_ && deleteBackup_)
                    File.Delete(backupFile);
            };            
            
            // Performs a standard write to a temporary file
            write(tempFile);            

            return this;
        }

        public Pnyx captureText(StringBuilder builder)
        {
            return setEnd(null, new CaptureText(streamInformation, builder));
        }

        public Pnyx tee(Action<Pnyx> block)
        {
            Pnyx teePnyx = new Pnyx(settings, streamInformation);
            
            if (state == FluentState.Start || state == FluentState.Line)
            {
                LinePassProcessor teePass = new LinePassProcessor();
                teePnyx.startLine(teePass);
                linePart(new LineTeeProcessor(teePass));
            }
            else if (state == FluentState.Row)
            {
                RowPassProcessor teePass = new RowPassProcessor();
                teePnyx.startRow(teePass, rowConverter);
                rowPart(new RowTeeProcessor(teePass));
            }
            else
                throw new IllegalStateException("Pnyx is not in Line, Row, or Start state: {0}", state.ToString());

            block(teePnyx);
            teePnyx.compile();
            resources.Add(teePnyx);
            return this;
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
            
            if (state == FluentState.CompiledServile)
                throw new IllegalStateException("Pnyx is servile to another Pnyx");
            
            if (state != FluentState.Compiled)
                throw new IllegalStateException("Pnyx must have an end point before processing");
                                        
            processor.process();
            state = FluentState.Processed;
            
            // Updates events
            if (stateProcessed != null)
                stateProcessed(this, this);
            stateProcessed = null;

            return this;
        }

        public void Dispose()
        {
            if (settings.processOnDispose && (state == FluentState.End || state == FluentState.Compiled))
                process();
            
            if (state == FluentState.Disposed)
                return;
            
            foreach (IDisposable resource in resources)
                resource.Dispose();                                    
            resources.Clear();

            stateProcessed = null;
            
            state = FluentState.Disposed;            
        }

        private ModifierType retrieveModifier<ModifierType>(bool stopAtFirstModifier = true, bool consume = false) 
            where ModifierType : class, IModifier 
        {
            // finds first modifier, scanning from end of parts
            for (int i = parts.Count - 1; i >= 0; i--)
            {
                IModifier modifier = parts[i] as IModifier;
                if (modifier == null)
                    continue;

                ModifierType result = modifier as ModifierType;
                if (stopAtFirstModifier || result != null)
                {
                    if (consume && result != null)
                        parts.RemoveAt(i);
                    
                    return result;
                }
            }

            return null;
        }

        // groups 0,1, or more filters.  Allows 0 and 1 so that any variable number of filters are treated as 1
        private Pnyx groupFilters(Action<Pnyx> block,
            Func<IEnumerable<ILineFilter>, ILineFilter> lineFilterFactory,
            Func<IEnumerable<IRowFilter>, IRowFilter> rowFilterFactory            
            )
        {
            FluentState current = state;
            
            if (state != FluentState.Line && state != FluentState.Row && state != FluentState.Start)
                throw new IllegalStateException("Pnyx is not in Line, Row, or Start state: {0}", state.ToString());

            int before = parts.Count;
            block(this);
            
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

        public Pnyx and(Action<Pnyx> block)
        {
            return groupFilters(block,
                x => new AndLineFilter(x),
                x => new AndRowFilter(x)
            );
        }

        public Pnyx or(Action<Pnyx> block)
        {
            return groupFilters(block,
                x => new OrLineFilter(x),
                x => new OrRowFilter(x)
            );
        }

        public Pnyx xor(Action<Pnyx> block)
        {
            return groupFilters(block,
                x => new XorLineFilter(x),
                x => new XorRowFilter(x)
            );
        }

        public Pnyx not(Action<Pnyx> block)
        {
            return groupFilters(block,
                x => new NotLineFilter(x),
                x => new NotRowFilter(x)
            );
        }

        public Pnyx beforeAfterFilter(int before, int after, Action<Pnyx> block)
        {
            and(block);

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

        public Pnyx sortLine(
            bool descending = false, 
            bool caseSensitive = false,
            String tempDirectory = null,
            int? bufferLines = null
            )
        {
            if (state != FluentState.Start && state != FluentState.Line)
                throw new IllegalStateException("Pnyx is not in Line,Row,Start state: {0}", state.ToString());

            tempDirectory = tempDirectory ?? settings.tempDirectory;
            bufferLines = bufferLines ?? settings.bufferLines;
            
            IComparer<String> comparer = new PnyxStringComparer(descending, caseSensitive);             
            LineSortProcessor sortProcessor = new LineSortProcessor(tempDirectory, comparer, bufferLines.Value);
            return linePart(sortProcessor);
        }

        public Pnyx sortRow(
            int[] columnNumbers = null,
            bool descending = false, 
            bool caseSensitive = false,
            String tempDirectory = null,
            int? bufferLines = null
            )
        {
            if (state != FluentState.Row)
                throw new IllegalStateException("Pnyx is not in Line,Row,Start state: {0}", state.ToString());           

            tempDirectory = tempDirectory ?? settings.tempDirectory;
            bufferLines = bufferLines ?? settings.bufferLines;

            columnNumbers = columnNumbers ?? new int[] { 1 };            
            List<RowComparer.ColumnDefinition> definitions = new List<RowComparer.ColumnDefinition>();
            foreach (int columnNumber in columnNumbers)
            {
                definitions.Add(new RowComparer.ColumnDefinition
                {
                    columnNumber = columnNumber,
                    comparer = new PnyxStringComparer(descending, caseSensitive)
                });
            }
            
            IComparer<String[]> comparer = new RowComparer(definitions);             
            RowSortProcessor sortProcessor = new RowSortProcessor(tempDirectory, comparer, bufferLines.Value);
            return rowPart(sortProcessor);
        }
    }
}