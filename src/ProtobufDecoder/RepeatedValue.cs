using System.Collections.Generic;

namespace ProtobufDecoder
{
    public class RepeatedValue : ProtobufValue<IEnumerable<ProtobufValue>>
    {
        public RepeatedValue(IEnumerable<ProtobufValue> values) : base(values)
        {
        }
    }
}