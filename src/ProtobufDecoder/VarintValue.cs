using System;
using System.ComponentModel;
using System.Globalization;

namespace ProtobufDecoder
{
    public class VarintValue : ProtobufValue
    {
        public VarintValue(byte[] varintBytes)
        {
            RawValue = varintBytes;
        }

        public override bool CanDecode => false;

        [Description("The boolean representation")]
        [ReadOnly(true)]
        [Browsable(true)]
        public string Bool
        {
            get
            {
                var result = ToTarget(RawValue, 64);

                if (result.Item1.HasValue)
                {
                    if (result.Item1.Value > 1)
                    {
                        return "Not a boolean";
                    }

                    if (result.Item1.Value == 1)
                    {
                        return Boolean.TrueString;
                    }

                    return Boolean.FalseString;
                }

                return "Not a boolean";
            }
        }

        [Description("The raw bytes that represent this Varint")]
        [ReadOnly(true)]
        [Browsable(true)]
        public byte[] RawBytes => RawValue;
        
        [Description("The unsigned 16-bit integer representation")]
        [ReadOnly(true)]
        [Browsable(true)]
        public string UInt16 => SafeConvert((ToTarget(RawValue, 16)));

        [Description("The unsigned 32-bit integer representation")]
        [ReadOnly(true)]
        [Browsable(true)]
        public string UInt32 => SafeConvert((ToTarget(RawValue, 32)));

        [Description("The unsigned 64-bit integer representation")]
        [ReadOnly(true)]
        [Browsable(true)]
        public string UInt64 => SafeConvert((ToTarget(RawValue, 64)));

        [Description("The signed 16-bit integer representation")]
        [ReadOnly(true)]
        [Browsable(true)]
        public string Int16 => SafeConvert((DecodeZigZag(ToTarget(RawValue, 16))));

        [Description("The signed 32-bit integer representation")]
        [ReadOnly(true)]
        [Browsable(true)]
        public string Int32 => SafeConvert((DecodeZigZag(ToTarget(RawValue, 32))));
        
        [Description("The signed 64-bit integer representation")]
        [ReadOnly(true)]
        [Browsable(true)]
        public string Int64 => SafeConvert((DecodeZigZag(ToTarget(RawValue, 64))));

        private static string SafeConvert(Tuple<ulong?, string> input)
        {
            if (input.Item1.HasValue)
            {
                return input.Item1.Value.ToString(CultureInfo.InvariantCulture);
            }

            return input.Item2;
        }

        private static string SafeConvert(Tuple<long?, string> input)
        {
            if (input.Item1.HasValue)
            {
                return input.Item1.Value.ToString(CultureInfo.InvariantCulture);
            }

            return input.Item2;
        }

        public int AsUInt32()
        {
            return Convert.ToInt32(ToTarget(RawValue, 32).Item1.Value);
        }

        // Decoding code below copied from here https://github.com/topas/VarintBitConverter/blob/b84ee7c953ff98b2043a2e58aa32624ff949bd43/src/VarintBitConverter/VarintBitConverter.cs#L185
        // Copyright notice:
        //
        // VarintBitConverter:
        // https://github.com/topas/VarintBitConverter 
        // Copyright (c) 2011 Tomas Pastorek, Ixone.cz. All rights reserved.
        //
        private static Tuple<long?, string> DecodeZigZag(Tuple<ulong?, string> input)
        {
            if (input.Item1 == null)
            {
                return new Tuple<long?, string>(null, input.Item2);
            }

            var value = input.Item1.Value;

            if ((value & 0x1) == 0x1)
            {
                return new Tuple<long?, string>((-1 * ((long)(value >> 1) + 1)), null);
            }

            return new Tuple<long?, string>((long)(value >> 1), null);
        }

        public static Tuple<ulong?, string> ToTarget(ReadOnlySpan<byte> bytes, int sizeBits)
        {
            var shift = 0;
            ulong result = 0;

            for (var index = 0; index < bytes.Length; index++)
            {
                ulong byteValue = bytes[index];
                ulong tmp = byteValue & 0x7f;
                result |= tmp << shift;

                if (shift > sizeBits)
                {
                    return new Tuple<ulong?, string>(null,  "Got too many bytes to represent this value");
                }

                if ((byteValue & 0x80) != 0x80)
                {
                    return new Tuple<ulong?, string>(result, null);
                }

                shift += 7;
            }

            return new Tuple<ulong?, string>(null, "Cannot decode varint from byte array.");
        }
    }
}