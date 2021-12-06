using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Google.Protobuf;

namespace ProtobufDecoder
{
    public abstract class ProtobufTag
    {
        private string _name;

        [Browsable(true)]
        [Description("The Protobuf wire type")]
        [ReadOnly(true)]
        public WireFormat.WireType WireType { get; set; }
        
        [Browsable(true)]
        [Description("The tag number in the serialized payload")]
        [DisplayName("Tag number")]
        [ReadOnly(true)]
        public int Index { get; set; }

        /// <summary>
        /// The parent of this tag
        /// </summary>
        /// <remarks>This is only set when this tag belogs to an embedded message. For tags in the main message this is always <c>null</c></remarks>
        [Browsable(false)]
        public ProtobufTag Parent { get; set; }

        /// <summary>
        /// Optional name of this tag
        /// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    return $"tag{Index}";
                }

                return _name;
            }
            set => _name = value;
        }

        [Browsable(true)]
        [Description("Indicates whether this tag is optional (value is true) or required (value is false)")]
        public bool IsOptional { get; set; } = false;
    }

    public class ProtobufTagSingle : ProtobufTag
    {
        [Browsable(true)]
        [Description("The value of this tag")]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ProtobufValue Value { get; set; }
        
        [Category("Offsets")]
        [Browsable(true)]
        [Description("Position in the byte stream where this tag starts")]
        [ReadOnly(true)]
        public int StartOffset { get; set; }

        [Category("Offsets")]
        [Browsable(true)]
        [Description("Position in the byte stream where this tag starts")]
        [ReadOnly(true)]
        public int TagOffset => StartOffset;
        
        [Category("Offsets")]
        [Browsable(true)]
        [Description("Position in the byte stream where the data of this tag starts")]
        [ReadOnly(true)]
        public int DataOffset { get; set; }
        
        [Category("Offsets")]
        [Browsable(true)]
        [Description("Length of the data of this tag")]
        [ReadOnly(true)]
        public int DataLength { get; set; }
        
        [Category("Offsets")]
        [Browsable(true)]
        [Description("Position in the byte stream where this tag ends")]
        [ReadOnly(true)]
        public int EndOffset { get; set; }

        [Category("Offsets")]
        [Browsable(true)]
        [Description("Number of bytes of this entire tag")]
        [ReadOnly(true)]
        public int Length => (EndOffset - StartOffset) + 1;
    }

    /// <summary>
    /// A virtual tag that contains all occurences of a tag number
    /// </summary>
    /// <remarks>Protobuf allows a tag number to appear multiple times, this type acts as a collection of those types and isn't a "real" tag itself</remarks>
    public class ProtobufTagRepeated : ProtobufTag
    {
        [Browsable(true)]
        [Description("The instances of this tag in the payload")]
        [ReadOnly(true)]
        public ObservableCollection<ProtobufTagSingle> Items { get; set; } = new ObservableCollection<ProtobufTagSingle>();
    }

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

            // Ensure parent is set on all child tags of this tag
            Tags = new ObservableCollection<ProtobufTag>(tags
                .Select(t =>
                {
                    t.Parent = this;
                    return t;
                })
                .ToList());
        }

        [Browsable(true)]
        [Description("The tags of this embedded message")]
        [ReadOnly(true)]
        public ObservableCollection<ProtobufTag> Tags { get; }
    }
}