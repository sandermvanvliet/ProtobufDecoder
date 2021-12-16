using Google.Protobuf;
using ProtobufDecoder.Values;

namespace ProtobufDecoder.Tags
{
    /// <summary>
    /// Represents a tag that contains packed varint values
    /// </summary>
    public class ProtobufTagPackedVarint : ProtobufTagPacked
    {
        public static ProtobufTagPackedVarint From(ProtobufTagSingle source)
        {
            // This clones the values from the original tag.
            return new ProtobufTagPackedVarint
            {
                IsOptional = source.IsOptional,
                Index = source.Index,
                Name = source.Name,
                Parent = source.Parent,
                WireType = WireFormat.WireType.Varint,
                Value = new PackedVarintValue(source.Value.RawValue),
                DataLength = source.DataLength,
                DataOffset = source.DataOffset,
                StartOffset = source.StartOffset,
                EndOffset = source.EndOffset
            };
        }
    }
}