namespace ProtobufDecoder
{
    public class VarintValue : ProtobufValue<int>
    {
        public VarintValue(int value) : base(value)
        {
        }
    }
}