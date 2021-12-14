using System.ComponentModel;

namespace ProtobufDecoder
{
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
        public int TagOffset => StartOffset;
        
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

        [Category("Offsets")]
        [Browsable(true)]
        [Description("Number of bytes of this entire tag")]
        [ReadOnly(true)]
        public int Length => (EndOffset - StartOffset) + 1;
    }
}