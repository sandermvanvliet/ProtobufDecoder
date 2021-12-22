using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using ProtobufDecoder.Application.Wpf.Annotations;

namespace ProtobufDecoder.Application.Wpf.Models
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        private string _inputFilePath;
        private string _windowTitle = "ProtobufDecoder";
        private string _outputFilePath;
        private string _statusBarText;
        private Brush _statusBarBackground = Brushes.DodgerBlue;
        private Brush _statusBarForeground = Brushes.White;
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

        public string StatusBarText
        {
            get => _statusBarText;
            private set
            {
                if (value == _statusBarText) return;
                _statusBarText = value;
                OnPropertyChanged();
            }
        }

        public Brush StatusBarBackground
        {
            get => _statusBarBackground;
            private set
            {
                if (Equals(value, _statusBarBackground)) return;
                _statusBarBackground = value;
                OnPropertyChanged();
            }
        }

        public Brush StatusBarForeground
        {
            get => _statusBarForeground;
            private set
            {
                if (Equals(value, _statusBarForeground)) return;
                _statusBarForeground = value;
                OnPropertyChanged();
            }
        }

        public void StatusBarInfo(string format, params object[] args)
        {
            StatusBarText = string.Format(format, args);
            StatusBarBackground = Brushes.DodgerBlue;
            StatusBarForeground = Brushes.White;
        }

        public void StatusBarWarning(string format, params object[] args)
        {
            StatusBarText = string.Format(format, args);
            StatusBarBackground = Brushes.DarkOrange;
            StatusBarForeground = Brushes.White;
        }

        public void StatusBarError(string format, params object[] args)
        {
            StatusBarText = string.Format(format, args);
            StatusBarBackground = Brushes.Red;
            StatusBarForeground = Brushes.White;
        }
    }
}