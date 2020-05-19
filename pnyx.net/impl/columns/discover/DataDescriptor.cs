using System;
using System.Collections.Generic;

namespace pnyx.net.impl.columns.discover
{
    public enum DataType
    {
        Blank,
        Unique,
        Enumerated,
        Formatted,
        Other 
    }

    public class DataDescriptor
    {
        public DataType type { get; private set; }
        public bool isUnique { get; private set; }
        public bool isFormatted { get; private set; }
        public bool isEnumerated { get; private set; }

        public List<String> enumValues { get; private set; }
        public String mask { get; private set; }

        public override string ToString()
        {
            switch (type)
            {
                default:
                case DataType.Blank:            return "Blank";
                case DataType.Enumerated:       return $"Enumerated({enumValues.Count})";
                case DataType.Formatted:        return $"FormattedData({mask})";
                case DataType.Other:            return "Other";
                case DataType.Unique:           return "Unique";
            }
        }

        private void setType(DataType theType)
        {
            if (type == DataType.Blank)
                type = theType;
        }

        public DataDescriptor setEnum(List<String> enumValues)
        {
            this.enumValues = enumValues;
            setType(DataType.Enumerated);
            isEnumerated = true;
            return this;
        }

        public DataDescriptor setFormatted(String mask)
        {
            this.mask = mask;
            setType(DataType.Formatted);
            isFormatted = true;
            return this;
        }

        public DataDescriptor setUnique()
        {
            setType(DataType.Unique);
            isUnique = true;
            return this;
        }

        public DataDescriptor setOther()
        {
            setType(DataType.Other);
            return this;
        }
    }
}