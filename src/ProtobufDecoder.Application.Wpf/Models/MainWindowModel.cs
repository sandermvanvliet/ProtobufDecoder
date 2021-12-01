using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using ProtobufDecoder.Application.Wpf.Annotations;

namespace ProtobufDecoder.Application.Wpf.Models
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        private string _inputFilePath;
        private ProtobufMessage _message;
        private string _windowTitle = "ProtobufDecoder";
        private Stream _inputFileByteStream;
        private string _outputFilePath;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string InputFilePath
        {
            get => _inputFilePath;
            set
            {
                if (value == _inputFilePath) return;
                _inputFilePath = value;
                OnPropertyChanged();

                WindowTitle = "ProtobufDecoder - " + value;
            }
        }

        public ProtobufMessage Message
        {
            get => _message;
            set
            {
                if (Equals(value, _message)) return;
                _message = value;
                OnPropertyChanged();
            }
        }

        public string WindowTitle
        {
            get => _windowTitle;
            set
            {
                if (value == _windowTitle) return;
                _windowTitle = value;
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

        public string OutputFilePath
        {
            get => _outputFilePath;
            set
            {
                if (value == _outputFilePath) return;
                _outputFilePath = value;
                OnPropertyChanged();
            }
        }

        public void DecodeTag(ProtobufTag tag)
        {
            if (tag is ProtobufTagSingle singleTag)
            {
                if(singleTag.Value.CanDecode)
                {
                    try
                    {
                        var parsedMessage = ProtobufParser.Parse(singleTag.Value.RawValue);
                        var embeddedMessageTag = new ProtobufTagEmbeddedMessage(singleTag, parsedMessage.Tags.ToArray())
                        {
                            Name = $"EmbeddedMessage{tag.Index}"
                        };

                        // Replace the existing tag with the expanded tag
                        if (singleTag.Parent is ProtobufTagRepeated repeatedTag)
                        {
                            var tagIndex = repeatedTag.Items.IndexOf(singleTag);
                            repeatedTag.Items.RemoveAt(tagIndex);
                            repeatedTag.Items.Insert(tagIndex, embeddedMessageTag);
                            OnPropertyChanged(nameof(Message));
                        }
                        else if(singleTag.Parent is ProtobufTagEmbeddedMessage embeddedTag)
                        {
                            var tagIndex = embeddedTag.Tags.IndexOf(singleTag);
                            embeddedTag.Tags.RemoveAt(tagIndex);
                            embeddedTag.Tags.Insert(tagIndex, embeddedMessageTag);
                            OnPropertyChanged(nameof(Message));
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine(exception);
                    }
                }
            }
        }
    }
}