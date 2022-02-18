using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using ProtobufDecoder.Application.Wpf.Annotations;
using ProtobufDecoder.Application.Wpf.Commands;
using ProtobufDecoder.Tags;

namespace ProtobufDecoder.Application.Wpf.ViewModels
{
    public class ProtobufTagViewModel : INotifyPropertyChanged, IProtobufParent
    {
        private bool _isExpanded;
        private bool _isSelected;
        private string _name;
        private IProtobufParent _parent;
        private ProtobufTag _tag;
        private ObservableCollection<ProtobufTagViewModel> _children = new();

        public ProtobufTagViewModel(ProtobufTag tag, IProtobufParent parent = null)
        {
            Name = tag.Name;
            Parent = parent;
            Tag = tag;

            PopulateChildren(tag);

            IsExpanded = Parent is MessageViewModel && Children.Any();
        }

        public bool CanDecode
        {
            get
            {
                if (Tag is ProtobufTagSingle singleTag && (singleTag.Value?.CanDecode ?? false))
                {
                    return true;
                }

                if (Tag is ProtobufTagRepeated repeated &&
                    repeated.Items.Count > 0 &&
                    repeated.Items.Any(tag => tag.Value?.CanDecode ?? false))
                {
                    return true;
                }

                if (Tag is ProtobufTagEmbeddedMessage embedded &&
                    embedded.Tags.Count > 0 &&
                    embedded.Tags.Any(tag => tag is ProtobufTagSingle single && (single.Value?.CanDecode ?? false)))
                {
                    return true;
                }

                return false;
            }
        }

        public ProtobufTag Tag
        {
            get => _tag;
            set
            {
                if (Equals(value, _tag))
                {
                    return;
                }

                if (_tag != null)
                {
                    Tag.PropertyChanged -= TagOnPropertyChanged;
                }

                _tag = value;
                Name = _tag.Name;

                OnPropertyChanged();
                OnPropertyChanged(nameof(CanDecode));
                OnPropertyChanged(nameof(MostLikelyValue));

                if (value != null)
                {
                    _tag.PropertyChanged += TagOnPropertyChanged;
                }
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
                if (value == _isSelected) return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (value == _isExpanded) return;
                _isExpanded = value;
                OnPropertyChanged();

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                {
                    _parent.IsExpanded = true;
                }
            }
        }

        public IProtobufParent Parent
        {
            get => _parent;
            set
            {
                if (Equals(value, _parent)) return;
                _parent = value;
                OnPropertyChanged();
            }
        }

        public string MostLikelyValue
        {
            get
            {
                if (Tag is ProtobufTagEmbeddedMessage embeddedMessage)
                {
                    return embeddedMessage.Name;
                }

                if (Tag is ProtobufTagRepeated repeated)
                {
                    return repeated.Name;
                }

                if (Tag is ProtobufTagSingle singleTag)
                {
                    return singleTag.Value?.ToString() ?? string.Empty;
                }

                return string.Empty;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void PopulateChildren(ProtobufTag tag)
        {
            IEnumerable<ProtobufTagViewModel> children = null;

            if (tag is ProtobufTagRepeated repeated)
            {
                children = repeated.Items.Select(item => new ProtobufTagViewModel(item, this));
            }
            else if (tag is ProtobufTagEmbeddedMessage embedded)
            {
                children = embedded.Tags.Select(item => new ProtobufTagViewModel(item, this));
            }

            // Clear event handler from children that we're removing
            foreach (var child in Children)
            {
                child.PropertyChanged -= ChildTagPropertyChanged;
            }

            // Clear the collection
            Children.Clear();

            if (children != null)
            {
                // Add new children
                foreach (var child in children)
                {
                    Children.Add(child);

                    // And listen for changes on the children
                    child.PropertyChanged += ChildTagPropertyChanged;
                }
            }
        }

        private void TagOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Because we're shadowing the actual tag with this view model
            // we need to update certain properties when they change on the
            // actual ProtobufTag instance.
            if (e.PropertyName == nameof(Name))
            {
                Name = Tag.Name;
            }
        }

        private void ChildTagPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Ignore IsExpanded and IsSelected events because they
            // are handled directly by the TreeView
            if (e.PropertyName == nameof(IsExpanded) || e.PropertyName == nameof(IsSelected))
            {
                return;
            }

            // Trigger an update of the Tag property so that
            // changes from child tags get bubbled up all the
            // way to the MessageViewModel.
            // That allows the UI to get notified of changes
            // down the tree of tags and update the generated
            // proto interface for example.
            OnPropertyChanged(nameof(Tag));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public CommandResult DecodeTag()
        {
            var parseResult = MessageParseResult.Failure("Can only decode a single tag");

            // Order matters in this check because an embedded message derives from a single tag
            if (Tag is ProtobufTagEmbeddedMessage)
            {
                var results = Children
                    .Where(c => c.CanDecode)
                    .Select(c => c.DecodeTag()).ToList();

                if (results.Any(r => r.Result == Result.Failure))
                {
                    parseResult = MessageParseResult.Failure("Some tags failed to decode: " + string.Join(", ", results.Where(r => r.Result == Result.Failure).Select(r => r.Message)));
                }
                else
                {
                    parseResult = MessageParseResult.Success(null);
                }
            }
            else if (Tag is ProtobufTagSingle singleTag)
            {
                parseResult = DecodeSingleTagDecodeTag(singleTag);
            }
            else if (Tag is ProtobufTagRepeated)
            {
                var results = Children
                    .Where(c => c.CanDecode)
                    .Select(c => c.DecodeTag()).ToList();

                if (results.Any(r => r.Result == Result.Failure))
                {
                    parseResult = MessageParseResult.Failure("Some tags failed to decode: " + string.Join(", ", results.Where(r => r.Result == Result.Failure).Select(r => r.Message)));
                }
                else
                {
                    parseResult = MessageParseResult.Success(null);
                }
            }

            return new CommandResult
            {
                Result = parseResult.Successful ? Result.Success : Result.Failure,
                Message = parseResult.FailureReason
            };
        }

        private MessageParseResult DecodeSingleTagDecodeTag(ProtobufTagSingle singleTag)
        {
            MessageParseResult parseResult;

            if (singleTag.Value.CanDecode)
            {
                if (Parent == null)
                {
                    return MessageParseResult.Failure("Tag does not have a parent");
                }

                try
                {
                    parseResult = ProtobufParser.Parse(singleTag.Value.RawValue);

                    if (parseResult.Successful)
                    {
                        var embeddedMessageTag =
                            new ProtobufTagEmbeddedMessage(singleTag, parseResult.Message.Tags.ToArray())
                            {
                                Name = $"EmbeddedMessage{Tag.Index}"
                            };

                        Parent.ReplaceChildWith(singleTag, embeddedMessageTag);
                        Tag = embeddedMessageTag;
                        PopulateChildren(embeddedMessageTag);
                        IsExpanded = true;
                    }
                }
                catch (Exception exception)
                {
                    parseResult = MessageParseResult.Failure($"Unexpected error: {exception.Message}");
                }
            }
            else
            {
                parseResult = MessageParseResult.Failure("Tag value can't be decoded");
            }

            return parseResult;
        }

        public void ReplaceChildWith(
            ProtobufTagSingle child,
            ProtobufTagSingle replacement)
        {
            if (Tag is ProtobufTagRepeated repeatedTag)
            {
                var tagIndex = repeatedTag.Items.IndexOf(child);
                repeatedTag.Items.RemoveAt(tagIndex);
                repeatedTag.Items.Insert(tagIndex, replacement);
            }
            else if (Tag is ProtobufTagEmbeddedMessage embeddedTag)
            {
                var tagIndex = embeddedTag.Tags.IndexOf(child);
                embeddedTag.Tags.RemoveAt(tagIndex);
                embeddedTag.Tags.Insert(tagIndex, replacement);
            }
            else
            {
                throw new InvalidOperationException($"A {Tag.Parent.GetType().Name} doesn't support child tags");
            }
        }

        public CommandResult CopyTagValueToCsharpArray()
        {
            if (Tag is ProtobufTagSingle singleTag and not ProtobufTagEmbeddedMessage)
            {
                var bytes = string.Join(", ", singleTag.Value.RawValue.Select(x => "0x" + x.ToString("X2").ToLower()));
                var csharpArray = $"var tagValueBytes = new byte[] {{{bytes}}};";

                Clipboard.SetText(csharpArray);

                return CommandResult.Success();
            }

            return CommandResult.Failure("Not a tag");
        }

        public CommandResult CopyMostLikelyValue()
        {
            Clipboard.SetText(MostLikelyValue);

            return CommandResult.Success();
        }
    }
}