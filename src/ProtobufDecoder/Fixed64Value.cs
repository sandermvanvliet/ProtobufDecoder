namespace ProtobufDecoder
{
    public class Fixed64Value : ProtobufValue<byte[]>
    {
        public Fixed64Value(byte[] value) : base(value)
        {
        }
    }
}