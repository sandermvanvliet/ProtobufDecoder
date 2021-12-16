using System;
using System.Collections.Generic;

namespace ProtobufDecoder.Values
{
    public class PackedFloatValue : ProtobufValue<float[]>
    {
        public PackedFloatValue(byte[] value) : base(ExplodeFloats(value))
        {
            RawValue = value;
        }

        public override bool CanDecode => false;

        private static float[] ExplodeFloats(ReadOnlySpan<byte> input)
        {
            var list = new List<float>();
            var index = 0;

            while (index < input.Length)
            {
                list.Add(new Fixed32Value(input.Slice(index, 4).ToArray()).Value);

                index += 4;
            }

            return list.ToArray();
        }
    }
}