using System.ComponentModel;
using System.Runtime.CompilerServices;
using Google.Protobuf;
using ProtobufDecoder.Annotations;

namespace ProtobufDecoder.Tags
{
    public abstract class ProtobufTag : INotifyPropertyChanged
    {
        private string _name;
        private bool _isOptional;

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
        [Browsable(true)]
        [Description("Name of this tag, defaults to tag{index}")]
        [ReadOnly(false)]
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
        [DisplayName("Optional")]
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
}