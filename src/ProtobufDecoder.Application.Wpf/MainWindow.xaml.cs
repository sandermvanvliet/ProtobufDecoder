using System.Windows;
using Microsoft.Win32;
using ProtobufDecoder.Application.Wpf.ViewModels;

namespace ProtobufDecoder.Application.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            DataContext = viewModel;

            InitializeComponent();

            ViewModel.Model.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(ViewModel.Model.Message))
                {
                    ViewModel.RenderProtoFileCommand.Execute(null);
                }
            };
        }

        private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

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

        private void TagsTreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var tag = TagsTreeView.SelectedItem as ProtobufTag;

            if (tag == null)
            {
                return;
            }

            if (tag is ProtobufTagSingle singleTag)
            {
                HexEditor.SelectionStart = singleTag.StartOffset;
                HexEditor.SelectionStop = singleTag.EndOffset;
            }
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.LoadFileCommand.CanExecute(ViewModel.Model.InputFilePath))
            {
                ViewModel.LoadFileCommand.Execute(ViewModel.Model.InputFilePath);
            }
        }

        private void OpenFileMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                RestoreDirectory = true,
                ShowReadOnly = true
            };

            var result = dialog.ShowDialog(this);

            if (result.Value)
            {
                ViewModel.Model.InputFilePath = dialog.FileName;

                if (ViewModel.LoadFileCommand.CanExecute(ViewModel.Model.InputFilePath))
                {
                    ViewModel.LoadFileCommand.Execute(ViewModel.Model.InputFilePath);
                }
            }
        }

        private void SaveGeneratedProto_OnClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}