using System.Text;

namespace ProtobufDecoder
{
    public class LengthDelimitedValue : ProtobufValue<byte[]>
    {
        public LengthDelimitedValue(byte[] value) : base(value)
        {
        }

        // Fun fact: you can't tell a string and a repeated packed varint value apart....
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