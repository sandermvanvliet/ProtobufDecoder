using System.ComponentModel;
using Google.Protobuf;

namespace ProtobufDecoder
{
    public class ProtobufTag
    {
        [Browsable(true)]
        [Description("The Protobuf wire type")]
        [ReadOnly(true)]
        public WireFormat.WireType WireType { get; set; }
        
        [Browsable(true)]
        [Description("The tag number in the serialized payload")]
        [DisplayName("Tag number")]
        [ReadOnly(true)]
        public int Index { get; set; }

        [Browsable(true)]
        [Description("The value of this tag")]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ProtobufValue Value { get; set; }
    }
}