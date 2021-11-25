using Google.Protobuf;

namespace ProtobufDecoder
{
    public class ProtobufTag
    {
        public WireFormat.WireType WireType { get; set; }
        public int Index { get; set; }
        public ProtobufValue Value { get; set; }
    }
}