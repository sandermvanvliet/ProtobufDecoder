using System.Windows;
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
            if (TagsTreeView.SelectedItem is ProtobufTagSingle singleTag)
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
    }
}