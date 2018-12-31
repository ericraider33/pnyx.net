using System;
using System.Text;
using pnyx.net.api;
using pnyx.net.processors;

namespace pnyx.net.impl
{
    public class Print : IRowProcessor, ILinePart, ILineProcessor
    {
        public String format;
        public ILineProcessor processor;
        public IRowConverter rowConverter;
        private readonly StringBuilder formatBuilder = new StringBuilder();
        private readonly String[] emptyRow = new String[0];

        public void rowHeader(String[] header)
        {
            String line = print(null, header);
            processor.processLine(line);            
        }

        public void processRow(String[] row)
        {
            String line = print(null, row);
            processor.processLine(line);            
        }

        public void processLine(String line)
        {
            line = print(line, emptyRow);
            processor.processLine(line);
        }

        public void endOfFile()
        {
            processor.endOfFile();            
        }

        public void setNextLineProcessor(ILineProcessor next)
        {
            processor = next;
        }
        
        private enum State { Text, Slash, Dollar, Number }
        
        public String print(String line, String[] row)
        {
            if (row == null)
                return null;

            formatBuilder.Clear();
            
            State state = State.Text;
            int column = 0;
            for (int i = 0; i < format.Length; i++)
            {
                char c = format[i];
                if (state == 0)
                {
                    switch (c)
                    {
                        case '$': state = State.Dollar; column = 0; break;
                        case '\\': state = State.Slash; break;
                        default: formatBuilder.Append(c); break;
                    }
                }
                else if (state == State.Slash)
                {
                    switch (c)
                    {
                        case '\\': formatBuilder.Append('\\'); break;
                        case 'n': formatBuilder.Append('\n'); break;
                        case 'r': formatBuilder.Append('\r'); break;
                        case 't': formatBuilder.Append('\t'); break;
                        default: formatBuilder.Append('\\').Append(c); break;
                    }
                    state = State.Text;
                }
                else if (state == State.Dollar)
                {
                    switch (c)
                    {
                        case '$': formatBuilder.Append('$'); break;
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            int value = c - '0';
                            column = column * 10 + value;
                            state = State.Number;
                            break;
                        
                        default:                            
                            formatBuilder.Append('$').Append(c);
                            state = State.Text;
                            break;
                    }
                }
                else if (state == State.Number)
                {
                    switch (c)
                    {
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            int value = c - '0';
                            column = column * 10 + value;
                            break;

                        default:
                            replace(column, ref line, row);

                            if (c == '$')
                            {
                                state = State.Dollar;
                                column = 0;
                            }
                            else if (c == '\\')
                            {
                                state = State.Slash;
                            }
                            else
                            {
                                formatBuilder.Append(c);
                                state = State.Text;                                
                            }
                            break;
                    }
                }
                
            }

            if (state == State.Dollar)
                formatBuilder.Append('$');
            else if (state == State.Slash)
                formatBuilder.Append('\\'); 
            else if (state == State.Number)
                replace(column, ref line, row);

            return formatBuilder.ToString();
        }

        private void replace(int column, ref String line, String[] row)
        {
            if (column == 0)
            {
                if (line == null)
                    line = rowConverter != null ? rowConverter.rowToLine(row) : "";
                
                formatBuilder.Append(line);                
            }
            else if (column <= row.Length)
            {
                formatBuilder.Append(row[column - 1]);
            }
        }
    }
}