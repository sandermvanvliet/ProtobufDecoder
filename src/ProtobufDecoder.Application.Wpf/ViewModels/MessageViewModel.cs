using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ProtobufDecoder.Application.Wpf.Annotations;

namespace ProtobufDecoder.Application.Wpf.ViewModels
{
    public class MessageViewModel : INotifyPropertyChanged
    {
        private ProtobufMessage _message;
        private ObservableCollection<ProtobufTagViewModel> _tags;

        public MessageViewModel(ProtobufMessage message)
        {
            Message = message;
            Tags = new ObservableCollection<ProtobufTagViewModel>(message.Tags.Select(tag => new ProtobufTagViewModel(tag)));
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