using System.Collections.Generic;
using System.ComponentModel;
using Google.Protobuf;
using ProtobufDecoder.Values;

namespace ProtobufDecoder.Tags
{
    public abstract class ProtobufTagPacked : ProtobufTagSingle
    {
        public static bool IsProbablePackedVarint(byte[] input)
        {
            var index = 0;

            try
            {
                while (index < input.Length)
                {
                    var parseResult = ProtobufParser.ParseVarint(input, index);
                
                    index += parseResult.Length;
                }

                if (index == input.Length)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Represents a tag that contains packed varint values
    /// </summary>
    public class ProtobufTagPackedVarint : ProtobufTagPacked
    {
        public static ProtobufTagPackedVarint PackedVarIntFrom(ProtobufTagSingle source)
        {
            // This clones the values from the original tag.
            return new ProtobufTagPackedVarint
            {
                IsOptional = source.IsOptional,
                Index = source.Index,
                Name = source.Name,
                Parent = source.Parent,
                WireType = WireFormat.WireType.Varint,
                Values = ExplodeVarInts(source.Value.RawValue),
                DataLength = source.DataLength,
                DataOffset = source.DataOffset,
                StartOffset = source.StartOffset,
                EndOffset = source.EndOffset
            };
        }

        private static VarintValue[] ExplodeVarInts(byte[] input)
        {
            var list = new List<VarintValue>();
            var index = 0;

            while (index < input.Length)
            {
                var parseResult = ProtobufParser.ParseVarint(input, index);

                list.Add(parseResult.Value);

                index += parseResult.Length;
            }

            return list.ToArray();
        }

        [Browsable(false)]
        public VarintValue[] Values { get; set; }
    }
}