using System;
using System.Linq;
using System.Text;
using pnyx.net.errors;
using pnyx.net.util;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace pnyx.cmd
{
    public class EncodingTypeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return typeof(Encoding).IsAssignableFrom(type);
        }

        public object ReadYaml(IParser parser, Type type)
        {            
            Scalar valueNode = parser.Expect<Scalar>();
            String valueText = valueNode.Value;
            
            EncodingInfo match = Encoding.GetEncodings().FirstOrDefault(enc => TextUtil.isEqualsIgnoreCase(enc.Name, valueText));
            if (match == null)
                throw new InvalidArgumentException("Could not convert text '{0}' to an encoding", valueText);
            
            return match.GetEncoding();
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            Encoding encoding = (Encoding) value;
            emitter.Emit(new Scalar(encoding.WebName));
        }
    }
}