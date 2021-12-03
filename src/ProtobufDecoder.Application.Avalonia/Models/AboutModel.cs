using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace ProtobufDecoder.Application.Avalonia.Models
{
    public class AboutModel : INotifyPropertyChanged
    {
        private string _version;

        public string Version
        {
            get => _version;
            set
            {
                if (value == _version)
                {
                    return;
                }

                _version = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AboutModel()
        {
            Version = GetType().Assembly.GetName().Version.ToString();
        }
    }
}