using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using pnyx.net.api;
using pnyx.net.errors;
using pnyx.net.transforms;
using pnyx.net.transforms.sed;
using pnyx.net.processors;

namespace pnyx.net.fluent
{
    public class Pnyx : IDisposable
    {
        private Stream start;
        private Stream end;
        private readonly ArrayList parts;
        private readonly List<IDisposable> resources;
        private IProcessor processor;
            
        public Pnyx()
        {            
            parts = new ArrayList();
            resources = new List<IDisposable>();
        }

        public Pnyx read(String path)
        {
            start = new FileStream(path, FileMode.Open, FileAccess.Read);
            return this;
        }

        public Pnyx grep(String textToFind, bool caseSensitive = true, bool invert = false)
        {
            parts.Add(new Grep { textToFind = textToFind, caseSensitive = caseSensitive, invert = invert });
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
            end = new FileStream(path, FileMode.Open, FileAccess.Write);
            compile();
            return this;
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
            
            LineProcessorToWriter lpEnd = new LineProcessorToWriter(end);
            ILineProcessor last = lpEnd; 
            for (int i = parts.Count-1; i >= 0; i--)
            {
                Object part = parts[i];

                ILineProcessor currentProcessor;
                                
                if (part is ILineFilter)                    
                    currentProcessor = new LineFilterProcessor { transform = (ILineFilter)part, processor = last };
                else if (part is ILineBuffering)
                    currentProcessor = new LineBufferingProcessor { transform = (ILineBuffering) part, processor = last};
                else
                    throw new NotImplementedException("Work in progress");

                last = currentProcessor;
            }

            processor = new ReaderToLineProcessor(start, last);
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