using System;
using System.IO;
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

        public ProtobufMessage Message { get; set; }

        public DependencyProperty InputFilePathProperty = DependencyProperty.Register(
            nameof(InputFilePath),
            typeof(string),
            typeof(MainWindow));

        public DependencyProperty RenderedProtoFileProperty = DependencyProperty.Register(
            nameof(RenderedProtoFile),
            typeof(string),
            typeof(MainWindow));

        public DependencyProperty InputFileByteStreamProperty = DependencyProperty.Register(
            nameof(InputFileByteStream),
            typeof(Stream),
            typeof(MainWindow));

        public string InputFilePath
        {
            get => (string)GetValue(InputFilePathProperty);
            set => SetValue(InputFilePathProperty, value);
        }

        public string RenderedProtoFile
        {
            get => (string)GetValue(RenderedProtoFileProperty);
            set => SetValue(RenderedProtoFileProperty, value);
        }

        public Stream InputFileByteStream
        {
            get => (Stream)GetValue(InputFileByteStreamProperty);
            set => SetValue(InputFileByteStreamProperty, value);
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


                // Ensure everything is cleared before we add something new.
                ClearWindow();

                if (string.IsNullOrEmpty(InputFilePath))
                {
                    ShowMessageBox.ForNoInputSelected();

                    return;
                }

                if (!File.Exists(InputFilePath))
                {
                    ShowMessageBox.ForFileDoesNotExist();

                    return;
                }

                try
                {
                    var bytes = File.ReadAllBytes(InputFilePath);
                    InputFileByteStream = new MemoryStream(bytes);
                    Decode(bytes);

                    RenderProtoFile(Message);
                }
                catch (IOException ioException)
                {
                    ShowMessageBox.ForReadingInputFailed(ioException);
                }
            }
        }

        private void RenderProtoFile(ProtobufMessage protobufMessage)
        {
            if (string.IsNullOrEmpty(Message.Name))
            {
                Message.Name = "Message";
            }

            RenderedProtoFile = ProtobufWriter.ToString(protobufMessage);
        }

        private void Decode(byte[] input)
        {
            try
            {
                Message = ProtobufParser.Parse(input);
            }
            catch (Exception exception)
            {
                ShowMessageBox.ForFailedToParseInput(exception);

                return;
            }

            TagsTreeView.ItemsSource = Message.Tags;
        }

        private void ClearWindow()
        {
            TagsTreeView.ItemsSource = null;
            GeneratedProtoTextBlock.Text = null;
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
    }
}