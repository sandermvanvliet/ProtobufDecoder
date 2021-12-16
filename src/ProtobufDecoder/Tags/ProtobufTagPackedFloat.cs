using System;
using System.Collections.Generic;
using System.ComponentModel;
using Google.Protobuf;
using ProtobufDecoder.Values;

namespace ProtobufDecoder.Tags
{
    /// <summary>
    /// Represents a tag that contains packed float (fixed32) values
    /// </summary>
    public class ProtobufTagPackedFloat : ProtobufTagPacked
    {
        public static ProtobufTagPackedFloat From(ProtobufTagSingle source)
        {
            // This clones the values from the original tag.
            return new ProtobufTagPackedFloat
            {
                IsOptional = source.IsOptional,
                Index = source.Index,
                Name = source.Name,
                Parent = source.Parent,
                WireType = WireFormat.WireType.Varint,
                Value = source.Value,
                Values = ExplodeFloats(source.Value.RawValue),
                DataLength = source.DataLength,
                DataOffset = source.DataOffset,
                StartOffset = source.StartOffset,
                EndOffset = source.EndOffset
            };
        }

        private static Fixed32Value[] ExplodeFloats(ReadOnlySpan<byte> input)
        {
            var list = new List<Fixed32Value>();
            var index = 0;

            while (index < input.Length)
            {
                list.Add(new Fixed32Value(input.Slice(index, 4).ToArray()));

                index += 4;
            }

            return list.ToArray();
        }

        [Browsable(false)]
        public Fixed32Value[] Values { get; set; }
    }
}