using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ProtobufDecoder.Application.Wpf.Annotations;
using ProtobufDecoder.Tags;

namespace ProtobufDecoder.Application.Wpf.ViewModels
{
    public class ProtobufTagViewModel : INotifyPropertyChanged
    {
        private bool _isExpanded;
        private bool _isSelected;
        private string _name;
        private ProtobufTagViewModel _parent;
        private ProtobufTag _tag;
        private ObservableCollection<ProtobufTagViewModel> _children = new();

        public ProtobufTagViewModel(ProtobufTag tag, ProtobufTagViewModel parent = null)
        {
            Name = tag.Name;
            Parent = parent;
            Tag = tag;

            PopulateChildren(tag);
        }

        private void PopulateChildren(ProtobufTag tag)
        {
            if (tag is ProtobufTagRepeated repeated)
            {
                var children = repeated.Items.Select(item => new ProtobufTagViewModel(item, this));

                Children = new ObservableCollection<ProtobufTagViewModel>(children);
            }
            else if (tag is ProtobufTagEmbeddedMessage embedded)
            {
                var children = embedded.Tags.Select(item => new ProtobufTagViewModel(item, this));

                Children = new ObservableCollection<ProtobufTagViewModel>(children);
            }
        }

        public bool CanDecode => Tag is ProtobufTagSingle singleTag && (singleTag.Value?.CanDecode ?? false);

        public ProtobufTag Tag
        {
            get => _tag;
            set
            {
                if (Equals(value, _tag))
                {
                    return;
                }

                _tag = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanDecode));
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name)
                {
                    return;
                }

                _name = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ProtobufTagViewModel> Children
        {
            get => _children;
            set
            {
                if (Equals(value, _children)) return;
                _children = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value == _isSelected)
                {
                    return;
                }

                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (value == _isExpanded)
                {
                    return;
                }

                _isExpanded = value;
                OnPropertyChanged();

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                {
                    _parent.IsExpanded = true;
                }
            }
        }

        public ProtobufTagViewModel Parent
        {
            get => _parent;
            set
            {
                if (Equals(value, _parent))
                {
                    return;
                }

                _parent = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MessageParseResult DecodeTag()
        {
            var parseResult = MessageParseResult.Failed("Can only decode a single tag");

            if (Tag is ProtobufTagSingle singleTag)
            {
                if(singleTag.Value.CanDecode)
                {
                    try
                    {
                        parseResult = ProtobufParser.Parse(singleTag.Value.RawValue);

                        if (parseResult.Success)
                        {
                            var embeddedMessageTag =
                                new ProtobufTagEmbeddedMessage(singleTag, parseResult.Message.Tags.ToArray())
                                {
                                    Name = $"EmbeddedMessage{Tag.Index}"
                                };

                            // Replace the existing tag with the expanded tag
                            if (singleTag.Parent is ProtobufTagRepeated repeatedTag)
                            {
                                var tagIndex = repeatedTag.Items.IndexOf(singleTag);
                                repeatedTag.Items.RemoveAt(tagIndex);
                                repeatedTag.Items.Insert(tagIndex, embeddedMessageTag);
                                Tag = embeddedMessageTag;
                                PopulateChildren(embeddedMessageTag);
                                IsExpanded = true;
                            }
                            else if (singleTag.Parent is ProtobufTagEmbeddedMessage embeddedTag)
                            {
                                var tagIndex = embeddedTag.Tags.IndexOf(singleTag);
                                embeddedTag.Tags.RemoveAt(tagIndex);
                                embeddedTag.Tags.Insert(tagIndex, embeddedMessageTag);
                                Tag = embeddedMessageTag;
                                PopulateChildren(embeddedMessageTag);
                                IsExpanded = true;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        parseResult = MessageParseResult.Failed($"Unexpected error: {exception.Message}");
                    }
                }
                else
                {
                    parseResult = MessageParseResult.Failed($"Tag value can't be decoded");
                }
            }

            return parseResult;
        }
    }
}