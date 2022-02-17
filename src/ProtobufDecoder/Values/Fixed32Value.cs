using System;
using System.ComponentModel;
using System.Globalization;

namespace ProtobufDecoder.Values
{
    public class Fixed32Value : ProtobufValue<float>
    {
        public Fixed32Value(byte[] value) : base(ParseFloat(value))
        {
            RawValue = value;
        }

        public override bool CanDecode => false;

        private static float ParseFloat(byte[] value)
        {
            return BitConverter.ToSingle(value);
        }

        [Description("The raw bytes that represent this Fixed32 value")]
        [ReadOnly(true)]
        [Browsable(true)]
        public byte[] RawBytes => RawValue;

        public override string ToString()
        {
            return Value.ToString(CultureInfo.CurrentUICulture);
        }
    }
}