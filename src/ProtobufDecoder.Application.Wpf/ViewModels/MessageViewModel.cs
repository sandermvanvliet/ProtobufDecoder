using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ProtobufDecoder.Application.Wpf.Annotations;
using ProtobufDecoder.Application.Wpf.Commands;

namespace ProtobufDecoder.Application.Wpf.ViewModels
{
    public class MessageViewModel : INotifyPropertyChanged
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
                Tags = new ObservableCollection<ProtobufTagViewModel>(_message.Tags.Select(tag => new ProtobufTagViewModel(tag)));
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

        public CommandResult LoadAndDecode(string inputFilePath)
        {
            // First clear the current message so that we don't show
            // stale data if loading this message fails.
            Message = new ProtobufMessage();

            try
            {
                var bytes = File.ReadAllBytes(inputFilePath);
                InputFileByteStream = new MemoryStream(bytes);
                var parseResult = ProtobufParser.Parse(bytes);

                if (parseResult.Successful)
                {
                    Message = parseResult.Message;

                    return CommandResult.Success();
                }

                return CommandResult.Failure(parseResult.FailureReason);
            }
            catch (Exception e)
            {
                return CommandResult.Failure(e.Message);
            }
        }
    }
}