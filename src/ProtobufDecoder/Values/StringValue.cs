using System.Text;

namespace ProtobufDecoder.Values
{
    public class StringValue : ProtobufValue<string>
    {
        public StringValue(byte[] rawValue) : base(Encoding.UTF8.GetString(rawValue))
        {
            RawValue = rawValue;
        }

        public override bool CanDecode => true; // Allow this for now as we don't have a good way to distinguish strings from embedded messages containing strings

        public override string ToString()
        {
            return string.IsNullOrEmpty(Value)
                ? "(empty)"
                : Value;
        }
    }
}