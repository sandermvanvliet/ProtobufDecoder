using System.ComponentModel;
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
        private string _renderedProtoFile;
        private Stream _inputFileByteStream;
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

        public string RenderedProtoFile
        {
            get => _renderedProtoFile;
            set
            {
                if (value == _renderedProtoFile) return;
                _renderedProtoFile = value;
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
    }
}