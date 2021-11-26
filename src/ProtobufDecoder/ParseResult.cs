namespace ProtobufDecoder
{
    internal class ParseResult<TProtoValue>
    {
        public int Length { get; set; }
        public TProtoValue Value { get; set; }
        public int DataOffset { get; set; }
    }
}