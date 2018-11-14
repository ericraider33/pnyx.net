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
        private StreamToRowProcessorDelegate rowReaderBuilder;
        private Stream end;        
        private readonly ArrayList parts;
        private readonly List<IDisposable> resources;
        private IProcessor processor;
        public StreamInformation streamInformation { get; private set; }        
            
        public Pnyx()
        {            
            streamInformation = new StreamInformation();
            parts = new ArrayList();
            resources = new List<IDisposable>();
        }

        public Pnyx read(String path)
        {
            start = new FileStream(path, FileMode.Open, FileAccess.Read);
            return this;
        }

        public Pnyx readStream(Stream input)
        {
            start = input;
            return this;
        }

        public Pnyx readString(String source)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);

            writer.Write(source);
            writer.Flush();
            stream.Position = 0;

            start = stream;
            
            return this;
        }

        public Pnyx rowCsv(bool strict = true)
        {
            rowReaderBuilder = (information, stream, rowProcessor) =>
            {
                CsvStreamToRowProcessor result = new CsvStreamToRowProcessor(information, stream, rowProcessor);
                result.setStrict(strict);                
                return result;
            };
            return this;
        }

        public Pnyx readCsv(String path, bool strict = true)
        {
            start = new FileStream(path, FileMode.Open, FileAccess.Read);
            rowReaderBuilder = (information, stream, rowProcessor) =>
            {
                CsvStreamToRowProcessor result = new CsvStreamToRowProcessor(information, stream, rowProcessor);
                result.setStrict(strict);                
                return result;
            };
            return this;            
        }

        public Pnyx grep(String textToFind, bool caseSensitive = true, bool invert = false)
        {
            parts.Add(new Grep { textToFind = textToFind, caseSensitive = caseSensitive, invert = invert });
            return this;
        }

        public Pnyx sed(String pattern, String replacement, String flags = null)
        {
            parts.Add(new SedReplace(pattern, replacement, flags));
            return this;
        }

        public Pnyx sedLineNumber()
        {
            parts.Add(new SedLineNumber());
            return this;
        }

        public Pnyx sedAppend(String text)
        {
            parts.Add(new SedAppend { text = text });
            return this;
        }

        public Pnyx sedInsert(String text)
        {
            parts.Add(new SedInsert { text = text });
            return this;
        }               

        public Pnyx write(String path)
        {
            end = new FileStream(path, FileMode.Create, FileAccess.Write);
            compile();
            return this;
        }

        public Pnyx writeStream(Stream output)
        {
            end = output;
            compile();
            return this;
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
            if (processor == null)
                throw new IllegalStateException("Pnyx must be compiled first by calling a 'write' method");
                
            processor.process();
            return this;
        }

        private void compile()
        {
            if (processor != null)
                throw new IllegalStateException("Pnyx has already been compiled");

            if (rowReaderBuilder != null)
                compileRowParts();
            else
                compileLineParts();
        }
        
        private void compileLineParts()
        {            
            LineProcessorToStream lpEnd = new LineProcessorToStream(streamInformation, end);
            ILineProcessor last = lpEnd; 
            for (int i = parts.Count-1; i >= 0; i--)
            {
                Object part = parts[i];

                ILineProcessor currentProcessor;
                                
                if (part is ILineFilter)                    
                    currentProcessor = new LineFilterProcessor { transform = (ILineFilter)part, processor = last };
                else if (part is ILineTransformer)
                    currentProcessor = new LineTransformerProcessor { transform = (ILineTransformer)part, processor = last };
                else if (part is ILineBuffering)
                    currentProcessor = new LineBufferingProcessor { transform = (ILineBuffering)part, processor = last};
                else
                    throw new NotImplementedException("Work in progress");

                last = currentProcessor;
            }

            processor = new StreamToLineProcessor(streamInformation, start, last);
        }

        private void compileRowParts()
        {         
            // Builds any shims
            shimLineParts();
            
            RowToCsvStream lpEnd = new RowToCsvStream(streamInformation, end);
            IRowProcessor last = lpEnd; 
            for (int i = parts.Count-1; i >= 0; i--)
            {
                Object part = parts[i];

                IRowProcessor currentProcessor;
                                
                if (part is IRowFilter)
                    currentProcessor = new RowFilterProcessor { transform = (IRowFilter)part, processor = last };
                else if (part is IRowTransformer)
                    currentProcessor = new RowTransformerProcessor { transform = (IRowTransformer)part, processor = last };
                else if (part is ILineBuffering)
                    currentProcessor = new RowBufferingProcessor { transform = (IRowBuffering)part, processor = last};
                else
                    throw new NotImplementedException("Work in progress");

                last = currentProcessor;
            }
            
            processor = rowReaderBuilder(streamInformation, start, last);            
        }

        private void shimLineParts()
        {
            for (int i = 0; i < parts.Count; i++)
            {
                Object part = parts[i];

                if (part is ILineFilter)
                {
                    if (!(part is IRowFilter))
                        parts[i] = new RowFilterShim { lineFilter = (ILineFilter)part };
                }
                else if (part is ILineTransformer)
                {
                    if (!(part is IRowTransformer))
                        parts[i] = new RowTransformerShim { lineTransformer = (ILineTransformer)part };
                }
                else if (part is ILineBuffering)
                {
                    if (!(part is IRowBuffering))
                        throw new NotImplementedException();
                        
//                        parts[i] = new RowBufferingShim(); // { lineBuffering = (ILineBuffering)part };
                }
            }
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

            rowReaderBuilder = null;
        }
    }
}