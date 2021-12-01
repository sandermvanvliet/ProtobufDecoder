using System.Windows;
using System.Windows.Controls;
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

        private void DecodeTagMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (TagsTreeView.SelectedItem is ProtobufTag tag)
            {
                if (tag is ProtobufTagSingle singleTag)
                {
                    if(singleTag.Value.CanDecode)
                    {
                        var treeViewItem = TagsTreeView.ItemContainerGenerator.ContainerFromItem(TagsTreeView.SelectedItem) as TreeViewItem;

                        var parsedMessage = ProtobufParser.Parse(singleTag.Value.RawValue);
                        var embeddedMessageTag = new ProtobufTagEmbeddedMessage(singleTag, parsedMessage.Tags.ToArray())
                        {
                            Name = $"EmbeddedMessage{tag.Index}"
                        };

                        // Replace the existing tag with the expanded tag
                        var parentTag = singleTag.Parent as ProtobufTagRepeated;
                        parentTag.Items.Remove(singleTag);
                        parentTag.Items.Add(embeddedMessageTag);

                        treeViewItem.ItemsSource = embeddedMessageTag.Tags;
                        treeViewItem.IsExpanded = true;

                        //TagsTreeView.ItemsSource = null;
                        //TagsTreeView.ItemsSource = ViewModel.Model.Message.Tags;

                        //treeViewItem.IsExpanded = true;
                    }
                }
            }
        }
    }
}