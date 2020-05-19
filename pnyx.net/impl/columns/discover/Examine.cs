using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pnyx.net.util;

namespace pnyx.net.impl.columns.discover
{
    public class Examine
    {
        public double enumPercentage = 0.20; 
        public int formattedUniqueLengths = 10; 
        
        private readonly Dictionary<int, int> lengthMap = new Dictionary<int, int>();
        private readonly Dictionary<String, int> enumMap = new Dictionary<String, int>();
        
        public DataDescriptor examine(List<String> data)
        {
            lengthMap.Clear();
            enumMap.Clear();

            // Initializes with first line of data
            List<char> common = data[0].Where(c => !Char.IsLetterOrDigit(c)).Distinct().ToList();
            
            foreach (String text in data)
            {
                lengthMap.increaseCount(text.Length);
                enumMap.increaseCount(text);
                
                // Finds char in common on every line
                for (int i = common.Count - 1; i >= 0; i--)
                {
                    char toCheck = common[i];
                    if (!text.Contains(toCheck))
                        common.RemoveAt(i);
                }
            }

            // Checks for length of data is mostly uniform
            if (lengthMap.Count < formattedUniqueLengths && common.Count > 0)
            {
                DataDescriptor formatted = examineFormatted(data);
                if (formatted != null)                                                    // continue to check remaining type if not considered formatted
                    return formatted;
            }

            if (enumMap.Count < data.Count * enumPercentage)
            {
                // NOTE: Still could be formatted with a lot of overlap, need to resolve

                List<String> enumValues = enumMap.Keys.ToList();
                return new DataDescriptor().setEnum(enumValues);
            }

            if (enumMap.Count == data.Count)
            {
                // NOTE: Still could be formatted with a lot of variety, need to resolve
                
                return new DataDescriptor().setUnique();
            }

            return new DataDescriptor().setOther();
        }

        public DataDescriptor examineFormatted(List<String> data)
        {
            List<MaskPart> parts = new List<MaskPart>();
            
            StringBuilder dataPart = new StringBuilder();
            StringBuilder tokenPart = new StringBuilder();
            foreach (String text in data)
            {
                int partIndex = 0;
                MaskPart part = null;
                
                foreach (char c in text)
                {
                    // Gets part
                    if (partIndex < parts.Count)
                        part = parts[partIndex];
                    else
                    {
                        part = new MaskPart();
                        parts.Add(part);
                    }
                    
                    // Reading data
                    if (dataPart.Length > 0)
                    {
                        if (Char.IsLetterOrDigit(c))
                        {
                            dataPart.Append(c);
                        }
                        else
                        {
                            // Process part
                            part.addPart(dataPart.ToString(), tokenPart.ToString());
                            partIndex++;

                            dataPart.Length = 0;
                            tokenPart.Length = 0;

                            tokenPart.Append(c);
                        }    
                    }
                    // Reading tokens
                    else
                    {
                        if (Char.IsLetterOrDigit(c))
                            dataPart.Append(c);
                        else                        
                            tokenPart.Append(c);
                    }
                }
                
                // Flushes final part
                if (tokenPart.Length > 0 || dataPart.Length > 0)
                    part.addPart(dataPart.ToString(), tokenPart.ToString());
                
                dataPart.Length = 0;
                tokenPart.Length = 0;
            }
            
            StringBuilder mask = new StringBuilder();
            foreach (MaskPart part in parts)
            {
                if (!part.isTokenConsistent())
                    return null;                                // assures that each line has tokens in same positions

                String token = part.getToken();
                if (token != null)
                {
                    //TODO - allow for optional parts
                    int tokenCount = part.getTokenCount();
                    if (tokenCount != data.Count)
                        return null;                            // for now, ignore masks that are only present some of the time 
                    
                    foreach (char c in token)
                        mask.Append('\\').Append(c);
                }

                mask.Append('[');
                if (part.hasLetters && part.hasNumbers)
                    mask.Append("\\w");
                else if (part.hasLetters)
                    mask.Append("a-zA-Z");
                else
                    mask.Append("\\d");
                mask.Append(']');

                if (!(part.dataLengthMin == 1 && part.dataLengthMax == 1))
                {
                    if (part.dataLengthMax > part.dataLengthMin)
                        mask.Append('{').Append(part.dataLengthMin).Append(',').Append(part.dataLengthMax).Append('}');
                    else
                        mask.Append('{').Append(part.dataLengthMin).Append('}');
                }
            }

            return new DataDescriptor().setFormatted(mask.ToString());
        }
    }
}