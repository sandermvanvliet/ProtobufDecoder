using System;
using System.ComponentModel;
using System.Globalization;

namespace ProtobufDecoder
{
    public class VarintValue : ProtobufValue<int>
    {
        private readonly byte[] _varintBytes;

        public VarintValue(byte[] varintBytes) : base(ConvertToInt32(varintBytes))
        {
            _varintBytes = varintBytes;
        }

        private static int ConvertToInt32(byte[] varintBytes)
        {
            var varintValue = 0;
            var shift = 0;

            foreach (var b in varintBytes)
            {
                var tmp = b & 0x7f;
                varintValue |= tmp << shift;

                shift += 7;
            }

            return varintValue;
        }

        [Description("The boolean representation")]
        [ReadOnly(true)]
        [Browsable(true)]
        public string Bool
        {
            get
            {
                if (Value == 0)
                {
                    return "false";
                }

                if (Value == 1)
                {
                    return "true";
                }

                return "Not a boolean";
            }
        }

        [Description("The raw bytes that represent this Varint")]
        [ReadOnly(true)]
        [Browsable(true)]
        public byte[] RawBytes => _varintBytes;
        
        [Description("The unsigned 16-bit integer representation")]
        [ReadOnly(true)]
        [Browsable(true)]
        public string UInt16 => SafeConvert(() => ((ushort)ToTarget(_varintBytes, 16)).ToString(CultureInfo.InvariantCulture));

        [Description("The unsigned 32-bit integer representation")]
        [ReadOnly(true)]
        [Browsable(true)]
        public string UInt32 => SafeConvert(() => ((uint)ToTarget(_varintBytes, 32)).ToString(CultureInfo.InvariantCulture));

        [Description("The signed 64-bit integer representation")]
        [ReadOnly(true)]
        [Browsable(true)]
        public string UInt64 => SafeConvert(() => ((ulong)ToTarget(_varintBytes, 64)).ToString(CultureInfo.InvariantCulture));

        [Description("The signed 16-bit integer representation")]
        [ReadOnly(true)]
        [Browsable(true)]
        public string Int16 => SafeConvert(() => ((short)DecodeZigZag(ToTarget(_varintBytes, 16))).ToString(CultureInfo.InvariantCulture));

        [Description("The signed 32-bit integer representation")]
        [ReadOnly(true)]
        [Browsable(true)]
        public string Int32 => SafeConvert(() => ((int)DecodeZigZag(ToTarget(_varintBytes, 32))).ToString(CultureInfo.InvariantCulture));

        
        [Description("The signed 64-bit integer representation")]
        [ReadOnly(true)]
        [Browsable(true)]
        public string Int64 => SafeConvert(() => ((long)DecodeZigZag(ToTarget(_varintBytes, 64))).ToString(CultureInfo.InvariantCulture));

        // Decoding code from here https://github.com/topas/VarintBitConverter/blob/b84ee7c953ff98b2043a2e58aa32624ff949bd43/src/VarintBitConverter/VarintBitConverter.cs#L185
        private static long DecodeZigZag(ulong value)
        {
            if ((value & 0x1) == 0x1)
            {
                return (-1 * ((long)(value >> 1) + 1));
            }

            return (long)(value >> 1);
        }
        
        private static ulong ToTarget(byte[] bytes, int sizeBits)
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
                    throw new ArgumentOutOfRangeException(nameof(bytes),  "Got too many bytes to represent this value");
                }

                if ((byteValue & 0x80) != 0x80)
                {
                    if ((index * 2) < (sizeBits / 8))
                    {
                        throw new ArgumentException("More bits were specified than there are bytes");
                    }

                    return result;
                }

                shift += 7;
            }

            throw new ArgumentException("Cannot decode varint from byte array.", nameof(bytes));
        }

        private static string SafeConvert(Func<string> getValueFunc)
        {
            try
            {
                return getValueFunc();
            }
            catch (ArgumentOutOfRangeException e)
            {
                return e.Message;
            }
            catch (ArgumentException e)
            {
                return e.Message;
            }
        }
    }
}