using System.ComponentModel;
using System.Text;

namespace ProtobufDecoder
{
    public class LengthDelimitedValue : ProtobufValue<byte[]>
    {
        public LengthDelimitedValue(byte[] value) : base(value)
        {
            RawValue = value;
        }

        public override bool CanDecode => true;

        // Fun fact: you can't tell a string and a repeated packed varint value apart....
        [Browsable(true)]
        [Description("The underlying value decoded as a UTF-8 string")]
        public string StringRepresentation
        {
            get
            {
                try
                {
                    return Encoding.UTF8.GetString(Value);
                }
                catch
                {
                    return "Doesn't seem to be a string";
                }
            }
        }
    }
}