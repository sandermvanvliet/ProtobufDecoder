using System;
using System.ComponentModel;
using System.Globalization;

namespace ProtobufDecoder.Values
{
    public class Fixed64Value : ProtobufValue<double>
    {
        public Fixed64Value(byte[] value) : base(ParseDouble(value))
        {
            RawValue = value;
        }

        public override bool CanDecode => false;

        private static double ParseDouble(byte[] value)
        {
            return BitConverter.ToDouble(value);
        }

        [Description("The raw bytes that represent this Fixed64 value")]
        [ReadOnly(true)]
        [Browsable(true)]
        public byte[] RawBytes => RawValue;

        public override string ToString()
        {
            return Value.ToString(CultureInfo.CurrentUICulture);
        }
    }
}