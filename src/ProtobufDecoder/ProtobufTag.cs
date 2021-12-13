using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Google.Protobuf;
using ProtobufDecoder.Annotations;

namespace ProtobufDecoder
{
    public abstract class ProtobufTag : INotifyPropertyChanged
    {
        private string _name;
        private bool _isOptional = false;

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
            set
            {
                if (_name == value)
                {
                    return;
                }

                _name = value;
                OnPropertyChanged();
            }
        }

        [Browsable(true)]
        [Description("Indicates whether this tag is optional (value is true) or required (value is false)")]
        [ReadOnly(true)]
        public bool IsOptional
        {
            get => _isOptional;
            set
            {
                if (value == _isOptional)
                {
                    return;
                }
                _isOptional = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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
        [Browsable(false)]
        [Description("The instances of this tag in the payload")]
        [ReadOnly(true)]
        public ObservableCollection<ProtobufTagSingle> Items { get; set; } = new ObservableCollection<ProtobufTagSingle>();
    }

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

            return tag;
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

        [Browsable(false)]
        [Description("The tags of this embedded message")]
        [ReadOnly(true)]
        public ObservableCollection<ProtobufTag> Tags { get; }
    }
}