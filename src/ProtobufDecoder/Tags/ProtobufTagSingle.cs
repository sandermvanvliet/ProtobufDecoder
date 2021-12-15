using System.ComponentModel;
using ProtobufDecoder.Values;

namespace ProtobufDecoder.Tags
{
    public class ProtobufTagSingle : ProtobufTag
    {
        [Browsable(true)]
        [Description("The value of this tag")]
        [ReadOnly(true)]
        [Category("Tag value")]
        public ProtobufValue Value { get; set; }
        
        [Category("Offsets")]
        [Browsable(true)]
        [Description("Position in the byte stream where this tag starts")]
        [ReadOnly(true)]
        [DisplayName("Start offset")]
        public int StartOffset { get; set; }

        [Category("Offsets")]
        [Browsable(true)]
        [Description("Position in the byte stream where this tag starts")]
        [ReadOnly(true)]
        [DisplayName("Tag offset")]
        public int TagOffset => StartOffset;
        
        [Category("Offsets")]
        [Browsable(true)]
        [Description("Position in the byte stream where the data of this tag starts")]
        [ReadOnly(true)]
        [DisplayName("Data offset")]
        public int DataOffset { get; set; }
        
        [Category("Offsets")]
        [Browsable(true)]
        [Description("Length of the data of this tag")]
        [ReadOnly(true)]
        [DisplayName("Data length")]
        public int DataLength { get; set; }
        
        [Category("Offsets")]
        [Browsable(true)]
        [Description("Position in the byte stream where this tag ends")]
        [ReadOnly(true)]
        [DisplayName("End offset")]
        public int EndOffset { get; set; }

        [Category("Offsets")]
        [Browsable(true)]
        [Description("Number of bytes of this entire tag")]
        [ReadOnly(true)]
        public int Length => (EndOffset - StartOffset) + 1;
    }
}