using System.Windows;
using Microsoft.Win32;

namespace ProtobufDecoder.Application.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        public DependencyProperty InputFilePathProperty = DependencyProperty.Register(
            nameof(InputFilePath), 
            typeof(string),
            typeof(MainWindow));

        public string InputFilePath
        {
            get => (string)GetValue(InputFilePathProperty);
            set => SetValue(InputFilePathProperty, value);
        }

        private void AboutMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new About
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            aboutWindow.ShowDialog();
        }

        private void ExitMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BrowseFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                RestoreDirectory = true,
                ShowReadOnly = true
            };

            var result = dialog.ShowDialog(this);

            if (result.HasValue && result.Value)
            {
                InputFilePath = dialog.FileName;
            }
        }
    }
}
