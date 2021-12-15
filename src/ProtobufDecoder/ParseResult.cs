namespace ProtobufDecoder
{
    public class ParseResult<TProtoValue>
    {
        public int Length { get; set; }
        public TProtoValue Value { get; set; }
        public int DataOffset { get; set; }
        public int DataLength { get; set; }
    }
}