using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using ProtobufDecoder.Application.Wpf.Annotations;
using ProtobufDecoder.Application.Wpf.Commands;
using ProtobufDecoder.Tags;

namespace ProtobufDecoder.Application.Wpf.ViewModels
{
    public class MessageViewModel : INotifyPropertyChanged, IProtobufParent
    {
        private ProtobufMessage _message;
        private ObservableCollection<ProtobufTagViewModel> _tags;
        private Stream _inputFileByteStream;

        public MessageViewModel()
        {
            Message = new ProtobufMessage();
        }

        public ObservableCollection<ProtobufTagViewModel> Tags
        {
            get => _tags;
            private set
            {
                if (Equals(value, _tags)) return;
                _tags = value;
                foreach (var tag in _tags)
                {
                    tag.PropertyChanged += TagPropertyChanged;
                }
                _tags.CollectionChanged += TagsOnCollectionChanged;
                OnPropertyChanged();
            }
        }

        private void TagsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged tag in e.OldItems)
                {
                    tag.PropertyChanged -= TagPropertyChanged;
                }
            }


            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged tag in e.NewItems)
                {
                    tag.PropertyChanged += TagPropertyChanged;
                }
            }
        }

        private void TagPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Ignore IsExpanded and IsSelected events because they
            // are handled directly by the TreeView
            if (e.PropertyName == nameof(ProtobufTagViewModel.IsSelected) ||
                e.PropertyName == nameof(ProtobufTagViewModel.IsExpanded))
            {
                return;
            }

            OnPropertyChanged(nameof(Message));
        }

        public ProtobufMessage Message
        {
            get => _message;
            private set
            {
                if (Equals(value, _message))
                {
                    return;
                }

                _message = value;
                Tags = new ObservableCollection<ProtobufTagViewModel>(_message.Tags.Select(tag => new ProtobufTagViewModel(tag, this)));
                OnPropertyChanged();
            }
        }

        public Stream InputFileByteStream
        {
            get => _inputFileByteStream;
            set
            {
                if (Equals(value, _inputFileByteStream)) return;
                _inputFileByteStream = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public CommandResult LoadAndDecodeFromClipboard()
        {
            if (Clipboard.GetData("application/octet-stream") is MemoryStream data)
            {
                var bytes = data.ToArray().Take((int)data.Length).ToArray();

                if (bytes.Length > 0)
                {
                    return Decode(bytes);
                }
            }

            return CommandResult.Failure(Strings.ClipboardEmpty);
        }

        public CommandResult LoadAndDecode(string inputFilePath)
        {
            var bytes = File.ReadAllBytes(inputFilePath);

            return Decode(bytes);
        }

        private CommandResult Decode(byte[] bytes)
        {
            // First clear the current message so that we don't show
            // stale data if loading this message fails.
            Message = new ProtobufMessage();

            try
            {
                InputFileByteStream = new MemoryStream(bytes);
                var parseResult = ProtobufParser.Parse(bytes);

                if (parseResult.Successful)
                {
                    Message = parseResult.Message;

                    return CommandResult.Success();
                }

                // Parsing failed, let's see if there is a length prefix.
                // That could either be 2 or 4 bytes, if the first two bytes
                // are zeros and the two bytes after that are not then it's
                // most likely a 4 byte prefix.
                if (bytes[0] == 0x0 && bytes[1] == 0x0 && (bytes[2] != 0x0 || bytes[3] != 0x0))
                {
                    // 4 byte prefix, figure out the length from the second pair of bytes
                    var length = ToUInt16(bytes, 2, 2);
                    if (length + 4 == bytes.Length)
                    {
                        // Bingo
                        parseResult = ProtobufParser.Parse(bytes.Skip(4).ToArray());

                        if (parseResult.Successful)
                        {
                            Message = parseResult.Message;

                            return CommandResult.SuccessWithWarning(string.Format(Strings.FileLengthPrefix, 4));
                        }
                    }
                }
                else
                {
                    // Try to figure out if the first two bytes are a length prefix
                    var length = ToUInt16(bytes, 0, 2);
                    if (length + 2 == bytes.Length)
                    {
                        // Bingo
                        parseResult = ProtobufParser.Parse(bytes.Skip(2).ToArray());

                        if (parseResult.Successful)
                        {
                            Message = parseResult.Message;

                            return CommandResult.SuccessWithWarning(string.Format(Strings.FileLengthPrefix, 2));
                        }
                    }
                }

                return CommandResult.Failure(parseResult.FailureReason);
            }
            catch (Exception e)
            {
                return CommandResult.Failure(e.Message);
            }
        }

        private static int ToUInt16(byte[] buffer, int start, int count)
        {
            if (buffer.Length >= start + count)
            {
                var b = buffer.Skip(start).Take(count).ToArray();
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(b);
                }

                if (b.Length == count)
                {
                    return (BitConverter.ToUInt16(b, 0));
                }

                return 0;
            }

            return 0;
        }

        // Ignore this
        public bool IsSelected { get; set; }
        // Ignore this
        public bool IsExpanded { get; set; }
        // Ignore this
        public IProtobufParent Parent { get; set; }

        public void ReplaceChildWith(ProtobufTagSingle child, ProtobufTagSingle replacement)
        {
            var tagIndex = Message.Tags.IndexOf(child);
            Message.Tags.RemoveAt(tagIndex);
            Message.Tags.Insert(tagIndex, replacement);
            Tags = new ObservableCollection<ProtobufTagViewModel>(Message.Tags.Select(tag => new ProtobufTagViewModel(tag, this)));           
        }
    }
}