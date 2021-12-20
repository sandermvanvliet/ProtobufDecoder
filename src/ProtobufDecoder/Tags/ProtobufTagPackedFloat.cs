using Google.Protobuf;
using ProtobufDecoder.Values;

namespace ProtobufDecoder.Tags
{
    /// <summary>
    /// Represents a tag that contains packed float (fixed32) values
    /// </summary>
    public class ProtobufTagPackedFloat : ProtobufTagPacked
    {
        public ProtobufTagPackedFloat()
        {
            WireType = WireFormat.WireType.Fixed32;
        }

        public static ProtobufTagPackedFloat From(ProtobufTagSingle source)
        {
            // This clones the values from the original tag.
            return new ProtobufTagPackedFloat
            {
                IsOptional = source.IsOptional,
                Index = source.Index,
                Name = source.Name,
                Parent = source.Parent,
                WireType = WireFormat.WireType.Fixed32,
                Value = new PackedFloatValue(source.Value.RawValue),
                DataLength = source.DataLength,
                DataOffset = source.DataOffset,
                StartOffset = source.StartOffset,
                EndOffset = source.EndOffset
            };
        }
    }
}