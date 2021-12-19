using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using ProtobufDecoder.Application.Wpf.ViewModels;
using ProtobufDecoder.Tags;
using WpfHexaEditor.Core;

namespace ProtobufDecoder.Application.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<SolidColorBrush> _selectionColors = new()
        {
            Brushes.LightGreen,
            Brushes.LightSkyBlue,
            Brushes.LightCoral,
            Brushes.LightCyan,
            Brushes.LightSalmon,
            Brushes.LightSeaGreen,
            Brushes.LightSlateGray
        };
        private int _selectionColorIndex;

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
            var viewModel = e.NewValue as ProtobufTagViewModel;

            if (viewModel == null)
            {
                return;
            }

            if (viewModel.Tag is ProtobufTagSingle singleTag)
            {
                var byteViewOffset = GetOffsetOf(singleTag);

                try
                {
                    if (singleTag.Parent is ProtobufTagEmbeddedMessage embeddedMessage)
                    {
                        var parentOffset = GetOffsetOf(embeddedMessage);

                        var embeddedMessageStartOffset = embeddedMessage.StartOffset + parentOffset;

                        var block = HexEditor.GetCustomBackgroundBlock(embeddedMessageStartOffset);

                        if (block == null)
                        {
                            // Only add a background block when it doesn't
                            // already exist.
                            HexEditor.CustomBackgroundBlockItems.Add(
                                new CustomBackgroundBlock(
                                    embeddedMessageStartOffset,
                                    embeddedMessage.Length,
                                    GetNextSelectionColor()));
                        }
                    }
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                }

                HexEditor.SelectionStart = singleTag.StartOffset + byteViewOffset;
                HexEditor.SelectionStop = singleTag.EndOffset + byteViewOffset;
            }
        }

        private SolidColorBrush GetNextSelectionColor()
        {
            if (_selectionColorIndex >= _selectionColors.Count)
            {
                // Roll around
                _selectionColorIndex = 0;
            }

            return _selectionColors[_selectionColorIndex++];
        }

        private static int GetOffsetOf(ProtobufTag tag, int offset = 0)
        {
            if (tag.Parent == null)
            {
                return 0;
            }

            tag = tag.Parent;

            if (tag is ProtobufTagEmbeddedMessage embeddedTag)
            {
                // Add the StartOffset of this tag
                return embeddedTag.DataOffset + GetOffsetOf(tag, offset);
            }

            if (tag is ProtobufTagSingle singleTag)
            {
                // Add the StartOffset of this tag
                return singleTag.StartOffset + GetOffsetOf(tag, offset);
            }

            if (tag is ProtobufTagRepeated)
            {
                // Don't add any additional offset because
                // a repeated tag is only a placeholder
                return offset + GetOffsetOf(tag, offset);
            }

            return offset;
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