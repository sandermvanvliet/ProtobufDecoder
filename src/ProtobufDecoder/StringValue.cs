namespace ProtobufDecoder
{
    public class StringValue : ProtobufValue<string>
    {
        public StringValue(string value) : base(value)
        {
        }

        public override bool CanDecode => false;
    }
}