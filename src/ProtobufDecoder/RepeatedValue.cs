using System.Collections.Generic;
using System.ComponentModel;

namespace ProtobufDecoder
{
    public class RepeatedValue : ProtobufValue<IEnumerable<ProtobufValue>>
    {
        public RepeatedValue(IEnumerable<ProtobufValue> values) : base(values)
        {
            Items = new ProtobufValueCollection(values);
        }

        [Browsable(true)]
        [ReadOnly(true)]
        [Description("The list of items")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ProtobufValueCollection Items { get; }
    }
}