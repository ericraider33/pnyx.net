using System;
using System.Collections;
using System.IO;
using pnyx.net.api;
using pnyx.net.filters;
using pnyx.net.processors;

namespace pnyx.net.fluent
{
    public class Pnyx
    {
        private Stream start;
        private Stream end;
        private readonly ArrayList parts;
            
        private Pnyx()
        {            
            parts = new ArrayList();
        }

        public static Pnyx read(String path)
        {
            Pnyx builder = new Pnyx();
            builder.start = new FileStream(path, FileMode.Open, FileAccess.Read);
            return builder;
        }

        public Pnyx grep(String textToFind, bool caseSensitive = true, bool invert = false)
        {
            parts.Add(new Grep { textToFind = textToFind, caseSensitive = caseSensitive, invert = invert });
            return this;
        }

        public IProcessor write(String path)
        {
            end = new FileStream(path, FileMode.Open, FileAccess.Write);
            return compile();
        }

        private IProcessor compile()
        {
            TextReader reader = new StreamReader(start);
            TextWriter writer = new StreamWriter(end);
            
            LineProcessorToWriter lpEnd = new LineProcessorToWriter { writer = writer };
            ILineProcessor last = lpEnd; 
            for (int i = parts.Count-1; i >= 0; i--)
            {
                var part = parts[i];
                
                ILineFilter current = part as ILineFilter;                
                LineFilterProcessor processor = new LineFilterProcessor { filter = current, processor = last };

                last = processor;
            }

            ReaderToLineProcessor result = new ReaderToLineProcessor { reader = reader, lineProcessor = last };
            return result;
        }
    }
}