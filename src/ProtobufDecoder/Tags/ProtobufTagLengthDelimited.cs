using System.Linq;
using Google.Protobuf;

namespace ProtobufDecoder.Tags
{
    /// <summary>
    /// A Protobuf tag that holds length-delimited data
    /// </summary>
    /// <remarks>The value of this tag can be a string or embedded message (<see cref="ProtobufTagEmbeddedMessage"/>)</remarks>
    public class ProtobufTagLengthDelimited : ProtobufTagSingle
    {
        public ProtobufTagLengthDelimited()
        {
            WireType = WireFormat.WireType.LengthDelimited;
        }

        public static ProtobufTagLengthDelimited From(ProtobufTagSingle source)
        {
            // This clones the values from the original tag.
            var tag = new ProtobufTagLengthDelimited
            {
                DataLength = source.DataLength,
                DataOffset = source.DataOffset,
                EndOffset = source.EndOffset,
                IsOptional = source.IsOptional,
                Index = source.Index,
                Name = source.Name,
                Parent = source.Parent,
                StartOffset = source.StartOffset,
                Value = source.Value,
                WireType = source.WireType
            };

            return tag;
        }

        public static bool IsProbableString(byte[] input)
        {
            var controlCharCount = input.Count(b => b <= 0x20 || b == 0x7f);
            var alnumCharCount = input.Count(b => char.IsLetterOrDigit((char)b));

            var isProbableString = controlCharCount / (float)input.Length < 0.1;
            
            if (isProbableString)
            {
                isProbableString = alnumCharCount / (float)input.Length > 0.5;
            }

            return isProbableString;
        }
    }
}