using System;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ProtobufDecoder.Tags
{
    /// <summary>
    /// A Protobuf tag that holds length-delimited data
    /// </summary>
    /// <remarks>The value of this tag can be a string or embedded message (<see cref="ProtobufTagEmbeddedMessage"/>)</remarks>
    public class ProtobufTagLengthDelimited : ProtobufTagSingle
    {
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
            
            if (IsProbableString(tag.Value.RawValue))
            {
                try
                {
                    var decodedMessage = ProtobufParser.Parse(tag.Value.RawValue);

                    if (decodedMessage.Tags.Any(t => t.Index <= 0))
                    {
                        // Valid tag indexes start at 1 to a very large number so
                        // any zero or negative values are out.
                        tag.PossibleEmbeddedMessage = false;
                        tag.PossibleString = true;
                        tag.StringValue = Encoding.UTF8.GetString(tag.Value.RawValue);
                    }
                    else
                    {
                        tag.PossibleEmbeddedMessage = true;
                        tag.PossibleString = false;
                    }
                }
                catch
                {
                    // Not an embedded protobuf message or it's malformed
                    tag.PossibleEmbeddedMessage = false;
                    tag.PossibleString = true;
                    tag.StringValue = Encoding.UTF8.GetString(tag.Value.RawValue);
                }
            }
            else
            {
                tag.PossibleEmbeddedMessage = false;
                tag.PossibleString = true;
                tag.StringValue = Encoding.UTF8.GetString(tag.Value.RawValue);
            }

            return tag;
        }

        public static bool IsProbableString(byte[] input)
        {
            var controlCharCount = input.Count(b => b <= 0x20 || b == 0x7f);
            var alnumCharCount = input.Count(b => Char.IsLetterOrDigit((char)b));

            var isProbableString = controlCharCount / (float)input.Length < 0.1;
            
            if (isProbableString)
            {
                isProbableString = alnumCharCount / (float)input.Length > 0.5;
            }

            return isProbableString;
        }
        
        [Category("Tag value")]
        [Browsable(true)]
        [ReadOnly(true)]
        [Description("The possible string value of this tag, null if it's not a string")]
        [DisplayName("String value")]
        public string StringValue { get; private set; }
        
        [Category("Tag value")]
        [Browsable(true)]
        [Description("Indicates whether the value is possibly a string")]
        [ReadOnly(true)]
        [DisplayName("Possibly string")]
        public bool PossibleString { get; private set; }
        
        [Category("Tag value")]
        [Browsable(true)]
        [Description("Indicates whether the value is possibly an embedded message")]
        [ReadOnly(true)]
        [DisplayName("Possibly embedded message")]
        public bool PossibleEmbeddedMessage { get; private set; }
    }
}