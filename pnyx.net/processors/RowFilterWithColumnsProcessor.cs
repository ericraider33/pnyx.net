using System;
using pnyx.net.api;

namespace pnyx.net.processors
{
    public class RowFilterWithColumnsProcessor : IRowPart, IRowProcessor
    {
        public int[] indexes;          
        public IRowFilter transform;
        public IRowProcessor processor;
    
        public void processRow(String[] row)
        {
            String[] toFilter = new String[indexes.Length];
            for (int i = 0; i < indexes.Length; i++)
            {
                int columnIndex = indexes[i];
                String column = columnIndex < row.Length ? row[columnIndex] : "";
                toFilter[i] = column;
            }
            
            if (transform.shouldKeepRow(toFilter))
                processor.processRow(row);
        }
    
        public void endOfFile()
        {
            processor.endOfFile();
        }
    
        public void setNext(IRowProcessor next)
        {
            processor = next;
        }        
    }
}