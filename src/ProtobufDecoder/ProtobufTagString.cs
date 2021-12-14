using System.ComponentModel;
using System.Text;
using ProtobufDecoder.Values;

namespace ProtobufDecoder
{
    /// <summary>
    /// A Protobuf tag that holds a string
    /// </summary>
    public class ProtobufTagString : ProtobufTagSingle
    {
        public static ProtobufTagString From(ProtobufTagSingle source)
        {
            // This clones the values from the original tag.

            return new ProtobufTagString
            {
                DataLength = source.DataLength,
                DataOffset = source.DataOffset,
                EndOffset = source.EndOffset,
                IsOptional = source.IsOptional,
                Index = source.Index,
                Name = source.Name,
                Parent = source.Parent,
                StartOffset = source.StartOffset,
                Value = new StringValue(Encoding.UTF8.GetString(source.Value.RawValue)),
                WireType = source.WireType,
            };
        }

        [Category("Tag value")]
        [Browsable(true)]
        [ReadOnly(true)]
        [DisplayName("String value")]
        public string StringValue => (Value as StringValue)?.Value;
    }
}