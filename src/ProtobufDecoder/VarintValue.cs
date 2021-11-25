using System;
using System.ComponentModel;

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

        [Description("The raw bytes that represent this Varint")]
        [ReadOnly(true)]
        [Browsable(true)]
        public byte[] RawBytes => _varintBytes;

        [Description("The unsigned 32-bit integer representation")]
        [ReadOnly(true)]
        [Browsable(true)]
        public uint UInt32 => (uint)ToTarget(_varintBytes, 32);
        
        [Description("The unsigned 16-bit integer representation")]
        [ReadOnly(true)]
        [Browsable(true)]
        public ushort UInt16 => (ushort)ToTarget(_varintBytes, 32);

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

        [Description("The signed 32-bit integer representation")]
        [ReadOnly(true)]
        [Browsable(true)]
        public int Int32 => (int)DecodeZigZag(ToTarget(_varintBytes, 32));

        [Description("The signed 16-bit integer representation")]
        [ReadOnly(true)]
        [Browsable(true)]
        public short Int16 => (short)DecodeZigZag(ToTarget(_varintBytes, 16));

        // Decoding code from here https://github.com/topas/VarintBitConverter/blob/b84ee7c953ff98b2043a2e58aa32624ff949bd43/src/VarintBitConverter/VarintBitConverter.cs#L185
        private static long DecodeZigZag(ulong value)
        {
            if ((value & 0x1) == 0x1)
            {
                return (-1 * ((long)(value >> 1) + 1));
            }

            return (long)(value >> 1);
        }
        
        private static ulong ToTarget(byte[] bytes, int sizeBites)
        {
            var shift = 0;
            ulong result = 0;

            foreach (ulong byteValue in bytes)
            {
                ulong tmp = byteValue & 0x7f;
                result |= tmp << shift;

                if (shift > sizeBites)
                {
                    throw new ArgumentOutOfRangeException(nameof(bytes), "Byte array is too large.");
                }

                if ((byteValue & 0x80) != 0x80)
                {
                    return result;
                }

                shift += 7;
            }

            throw new ArgumentException("Cannot decode varint from byte array.", nameof(bytes));
        }
    }
}