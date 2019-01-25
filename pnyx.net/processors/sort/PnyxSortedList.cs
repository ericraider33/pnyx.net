using System;
using System.Collections.Generic;

namespace pnyx.net.processors.sort
{
    public class PnyxSortedList<T> where T : class
    {
        public int count { get; private set; }
        private readonly SortedList<T, int> buffer;
        private readonly bool unique;
        
        public PnyxSortedList(int bufferSize, IComparer<T> comparer, bool unique)
        {
            buffer = new SortedList<T, int>(bufferSize, comparer);
            this.unique = unique;
        }

        public void add(T toAdd)
        {                        
            if (buffer.ContainsKey(toAdd))
            {
                if (unique)
                    return;            // ignore duplicates

                int nodeCount = buffer[toAdd];
                buffer[toAdd] = nodeCount + 1;
                count++;
            }
            else
            {
                buffer.Add(toAdd, 1);
                count++;
            }
        }

        public void clear()
        {
            count = 0;
            buffer.Clear();
        }

        public void visit(Action<T> vistor)
        {
            foreach (KeyValuePair<T, int> node in buffer)
            {
                for (int i = 0; i < node.Value; i++)
                    vistor(node.Key);
            }
        }
    }
}