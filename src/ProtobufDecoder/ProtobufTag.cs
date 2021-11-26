using System.ComponentModel;
using Google.Protobuf;

namespace ProtobufDecoder
{
    public abstract class ProtobufTag
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
    }

    public class ProtobufTagSingle : ProtobufTag
    {
        [Browsable(true)]
        [Description("The value of this tag")]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ProtobufValue Value { get; set; }
        
        [Category("Offsets")]
        [Browsable(true)]
        [Description("Position in the byte stream where this tag starts")]
        [ReadOnly(true)]
        public int StartOffset { get; set; }
        
        [Category("Offsets")]
        [Browsable(true)]
        [Description("Position in the byte stream where this tag starts")]
        [ReadOnly(true)]
        public int TagOffset { get; set; }
        
        [Category("Offsets")]
        [Browsable(true)]
        [Description("Position in the byte stream where the data of this tag starts")]
        [ReadOnly(true)]
        public int DataOffset { get; set; }
        
        [Category("Offsets")]
        [Browsable(true)]
        [Description("Length of the data of this tag")]
        [ReadOnly(true)]
        public int DataLength { get; set; }
        
        [Category("Offsets")]
        [Browsable(true)]
        [Description("Position in the byte stream where this tag ends")]
        [ReadOnly(true)]
        public int EndOffset { get; set; }
    }

    public class ProtobufTagRepeated : ProtobufTag
    {
        [Browsable(true)]
        [Description("The instances of this tag in the payload")]
        [ReadOnly(true)]
        public ProtobufTagSingle[] Items { get; set; }
    }
}