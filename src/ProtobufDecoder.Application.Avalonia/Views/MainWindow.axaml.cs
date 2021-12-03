using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using ProtobufDecoder.Application.Avalonia.ViewModels;

namespace ProtobufDecoder.Application.Avalonia.Views
{
    public partial class MainWindow : Window
    {
        private readonly List<ISolidColorBrush> _selectionColors = new()
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

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private MainWindowViewModel ViewModel => DataContext as MainWindowViewModel;

        private void AboutMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            //var aboutWindow = new About
            //{
            //    Owner = this,
            //    WindowStartupLocation = WindowStartupLocation.CenterOwner
            //};

            //aboutWindow.ShowDialog();
        }

        private void ExitMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TagsTreeView_OnSelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            var tagsTreeView = sender as TreeView;

            if (tagsTreeView?.SelectedItem is ProtobufTagSingle singleTag)
            {
                var byteViewOffset = GetOffsetOf(singleTag);

                try
                {
                    if (singleTag.Parent is ProtobufTagEmbeddedMessage embeddedMessage)
                    {
                        var parentOffset = GetOffsetOf(embeddedMessage);

                        var embeddedMessageStartOffset = embeddedMessage.StartOffset + parentOffset;

                        //var block = HexEditor.GetCustomBackgroundBlock(embeddedMessageStartOffset);

                        //if (block == null)
                        //{
                        //    // Only add a background block when it doesn't
                        //    // already exist.
                        //    HexEditor.CustomBackgroundBlockItems.Add(
                        //        new CustomBackgroundBlock(
                        //            embeddedMessageStartOffset,
                        //            embeddedMessage.Length,
                        //            GetNextSelectionColor()));
                        //}
                    }
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                }

                //HexEditor.SelectionStart = singleTag.StartOffset + byteViewOffset;
                //HexEditor.SelectionStop = singleTag.EndOffset + byteViewOffset;
            }
        }

        private ISolidColorBrush GetNextSelectionColor()
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

        private void StyledElement_OnDataContextChanged(object? sender, EventArgs e)
        {
            if (ViewModel.LoadFileCommand.CanExecute(ViewModel.Model.InputFilePath))
            {
                ViewModel.LoadFileCommand.Execute(ViewModel.Model.InputFilePath);
            }   
        }
    }
}
