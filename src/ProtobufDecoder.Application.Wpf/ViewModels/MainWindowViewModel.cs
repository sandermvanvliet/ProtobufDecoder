using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using ProtobufDecoder.Application.Wpf.Commands;
using ProtobufDecoder.Application.Wpf.Models;
using ProtobufDecoder.Tags;

namespace ProtobufDecoder.Application.Wpf.ViewModels
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            Model = new MainWindowModel();

            LoadFileCommand = new RelayCommand(
                _ => LoadAndDecode(Model.InputFilePath),
                _ => !string.IsNullOrEmpty(Model?.InputFilePath));

            OpenFileCommand = new RelayCommand(
                _ => OpenFile(),
                _ => true);

            SaveGeneratedProtoCommand = new RelayCommand(
                _ => SaveGeneratedProtoFile(),
                _ => Model?.Message != null);

            SaveGeneratedProtoAsCommand = new RelayCommand(
                _ => SaveGeneratedProtoFileAs(),
                _ => Model?.Message != null);

            CopyTagValueCommand = new RelayCommand(
                _ => CopyTagValueToCsharpArray((_ as  TreeView)?.SelectedItem as ProtobufTag),
                _ => (_ as TreeView)?.SelectedItem != null);

            DecodeTagCommand = new RelayCommand(
                _ => DecodeTag((_ as TreeView)?.SelectedItem as ProtobufTag),
                _ => (_ as TreeView)?.SelectedItem != null);
        }

        public ICommand LoadFileCommand { get; }
        public ICommand OpenFileCommand { get; set; }
        public ICommand SaveGeneratedProtoCommand { get; }
        public ICommand SaveGeneratedProtoAsCommand { get; }
        public ICommand CopyTagValueCommand { get; set; }
        public ICommand DecodeTagCommand { get; set; }

        public MainWindowModel Model { get; set; }

        private void LoadAndDecode(string inputFilePath)
        {
            try
            {
                var bytes = File.ReadAllBytes(inputFilePath);
                Model.InputFileByteStream = new MemoryStream(bytes);
                var parseResult = ProtobufParser.Parse(bytes);

                if (parseResult.Success)
                {
                    Model.Message = parseResult.Message;
                    Model.StatusBarInfo(Strings.FileLoadedSuccessfully);
                }
                else
                {
                    Model.StatusBarError(Strings.FileFailedToLoad, parseResult.FailureReason);
                }
            }
            catch (Exception e)
            {
                Model.StatusBarError(Strings.FileFailedToLoad, e.Message);
            }
        }

        private void OpenFile()
        {
            var dialog = new OpenFileDialog
            {
                RestoreDirectory = true,
                CheckFileExists = true,
                ShowReadOnly = true
            };

            var result = dialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                Model.InputFilePath = dialog.FileName;

                LoadFileCommand.Execute(null);
            }
        }

        private void SaveGeneratedProtoFile()
        {
            if (string.IsNullOrEmpty(Model.OutputFilePath))
            {
                var dialog = new SaveFileDialog
                {
                    RestoreDirectory = true,
                    AddExtension = true,
                    DefaultExt = ".proto",
                    Filter = Strings.ProtoFileType
                };

                var result = dialog.ShowDialog();

                if (!result.HasValue || !result.Value)
                {
                    return;
                }

                Model.OutputFilePath = dialog.FileName;
            }

            try
            {
                File.WriteAllText(Model.OutputFilePath, ProtobufWriter.ToString(Model.Message));
                Model.StatusBarInfo(Strings.ProtoFileSavedSuccessfully);
            }
            catch (Exception e)
            {
                Model.StatusBarError(Strings.ProtoFileFailedToSave, e.Message);
            }
        }

        private void SaveGeneratedProtoFileAs()
        {
            var dialog = new SaveFileDialog
            {
                RestoreDirectory = true,
                AddExtension = true,
                DefaultExt = ".proto",
                Filter = Strings.ProtoFileType
            };

            var result = dialog.ShowDialog();

            if (!result.HasValue || !result.Value)
            {
                return;
            }

            try
            {
                File.WriteAllText(dialog.FileName, ProtobufWriter.ToString(Model.Message));
                Model.StatusBarInfo(Strings.ProtoFileSavedAs, dialog.FileName);
            }
            catch (Exception e)
            {
                Model.StatusBarError(Strings.ProtoFileFailedToSave, e.Message);
            }
        }

        private void CopyTagValueToCsharpArray(ProtobufTag selectedTag)
        {
            if (selectedTag is ProtobufTagSingle singleTag and not ProtobufTagEmbeddedMessage)
            {
                var bytes = string.Join(", ", singleTag.Value.RawValue.Select(x => "0x" + x.ToString("X2").ToLower()));
                var csharpArray = $"var tagValueBytes = new byte[] {{{bytes}}};";

                Clipboard.SetText(csharpArray);

                Model.StatusBarInfo(Strings.TagCopiedToClipboard);
            }
        }

        private void DecodeTag(ProtobufTag selectedTag)
        {
            if (selectedTag is ProtobufTagSingle singleTag and not ProtobufTagEmbeddedMessage)
            {
                if (singleTag.Value.CanDecode)
                {
                    var parseResult = Model.DecodeTag(selectedTag);
                    if (parseResult.Success)
                    {
                        Model.StatusBarInfo(Strings.TagDecodedSuccessfully);
                    }
                    else
                    {
                        Model.StatusBarError(Strings.FailedToDecodeTag, parseResult.FailureReason);
                    }

                    return;
                }
            }

            Model.StatusBarWarning(Strings.CannotDecodeTag);
        }

        public void SetSelectedTagProperty(ProtobufTag tag, string propertyName, object value)
        {
            Model.SetTagProperty(tag, propertyName, value);
        }
    }
}