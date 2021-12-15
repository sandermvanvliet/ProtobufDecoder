using System.Text;

namespace ProtobufDecoder.Values
{
    public class StringValue : ProtobufValue<string>
    {
        public StringValue(byte[] rawValue) : base(Encoding.UTF8.GetString(rawValue))
        {
            RawValue = rawValue;
        }

        public override bool CanDecode => false;
    }
}