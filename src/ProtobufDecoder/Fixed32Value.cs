using System;
using System.ComponentModel;

namespace ProtobufDecoder
{
    public class Fixed32Value : ProtobufValue<float>
    {
        public Fixed32Value(byte[] value) : base(ParseFloat(value))
        {
            RawBytes = value;
        }

        private static float ParseFloat(byte[] value)
        {
            return BitConverter.ToSingle(value);
        }

        [Description("The raw bytes that represent this Fixed32 value")]
        [ReadOnly(true)]
        [Browsable(true)]
        public byte[] RawBytes {get; }
    }
}