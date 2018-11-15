using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using pnyx.net.api;
using pnyx.net.errors;
using pnyx.net.transforms;
using pnyx.net.transforms.sed;
using pnyx.net.processors;
using pnyx.net.shims;
using pnyx.net.util;

namespace pnyx.net.fluent
{
    public class Pnyx : IDisposable
    {
        private Stream start;
        private Stream end;        
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
                return this;
            }
            else
                throw new IllegalStateException("Pnyx is not in Line, Row, or Start state: {0}", state.ToString());
        }

        public Pnyx rowCsv(bool strict = true)
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
            if (state == FluentState.Row)
            {
                if (filter is IRowFilter)
                    return rowFilter((IRowFilter) filter);
                else
                    return rowFilter(new RowFilterShim { lineFilter = filter });                
            }
                
            return linePart(new LineFilterProcessor { transform = filter });
        }
        
        public Pnyx lineTransformer(ILineTransformer transform)
        {
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

        public Pnyx rowFilter(IRowFilter transform)
        {
            return rowPart(new RowFilterProcessor { transform = transform });
        }

        public Pnyx rowTransformer(IRowTransformer transform)
        {
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
            if (state == FluentState.New || state == FluentState.End)
                throw new IllegalStateException("Pnyx is not in Line, Row, or Start state: {0}", state.ToString());

            end = output;

            IRowConverter endRowConverter = destination as IRowConverter;
            ILineProcessor lineDestination = destination as ILineProcessor;
            
            if ((state == FluentState.Start || state == FluentState.Line) && endRowConverter != null)
            {
                //TODO insert a lineToRow conversion and then compile as ROW
                throw new NotImplementedException("TODO");    
            }

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
                {
                    //TODO insert a rowToLine conversion and drop to line
                    throw new NotImplementedException("TODO");    
                    //state = FluentState.Line;
                }
                else
                    compile(rowDestination);
            }

            if (state == FluentState.Line || state == FluentState.Start)
            {
                lineDestination = lineDestination ?? new LineProcessorToStream(streamInformation, end);
                compile(lineDestination);
            }
            
            return this;
        }

        private void compile(Object destination)
        {                     
            Object last = destination; 
            for (int i = parts.Count-1; i >= 0; i--)
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
                    throw new IllegalStateException("Unknown part {0}", part.GetType().Name);

                last = part;                    
            }

            if (last is IRowProcessor)
            {
                rowSource.setSource(streamInformation, start, (IRowProcessor)last);
                processor = rowSource;
            }
            else if (last is ILineProcessor)
            {
                processor = new StreamToLineProcessor(streamInformation, start, (ILineProcessor)last);
            }
            else if (last == null)
            {
                last = new LineProcessorToStream(streamInformation, end);
                processor = new StreamToLineProcessor(streamInformation, start, (ILineProcessor)last);
            }
            else
                throw new IllegalStateException("Unknown part {0}", last.GetType().Name);                        
            
            state = FluentState.End;
        }        

        public Pnyx write(String path)
        {
            return setEnd(new FileStream(path, FileMode.Create, FileAccess.Write));
        }

        public Pnyx writeStream(Stream output)
        {
            return setEnd(output);
        }

        public String processToString()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                writeStream(stream);
                processor.process();

                return streamInformation.encoding.GetString(stream.ToArray());
            }
        }

        public Pnyx process()
        {
            if (state != FluentState.End)
                throw new IllegalStateException("Pnyx must be fully compiled");
                
            processor.process();
            return this;
        }

        public void Dispose()
        {
            foreach (IDisposable resource in resources)
                resource.Dispose();                                    
            resources.Clear();
            
            if (start != null)
                start.Dispose();
            start = null;

            if (end != null)
                end.Dispose();
            end = null;
        }
    }
}