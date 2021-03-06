using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Google.Protobuf;

namespace ProtobufDecoder.Tags
{
    /// <summary>
    /// A Protobuf tag that is an embedded message
    /// </summary>
    /// <remarks>This type holds the tags of the embedded message</remarks>
    public class ProtobufTagEmbeddedMessage : ProtobufTagSingle
    {
        public ProtobufTagEmbeddedMessage(ProtobufTagSingle tag, ProtobufTag[] tags)
        {
            Index = tag.Index;
            StartOffset = tag.StartOffset;
            DataOffset = tag.DataOffset;
            DataLength = tag.DataLength;
            EndOffset = tag.EndOffset;
            Parent = tag.Parent;

            // Wire type for embedded messages is length-delimited,
            // see https://developers.google.com/protocol-buffers/docs/encoding#structure
            WireType = WireFormat.WireType.LengthDelimited;

            // Ensure parent is set on all child tags of this tag
            Tags = new ObservableCollection<ProtobufTag>(tags
                .Select(t =>
                {
                    t.Parent = this;
                    return t;
                })
                .ToList());
        }

        [Browsable(false)]
        public ObservableCollection<ProtobufTag> Tags { get; }
    }
}