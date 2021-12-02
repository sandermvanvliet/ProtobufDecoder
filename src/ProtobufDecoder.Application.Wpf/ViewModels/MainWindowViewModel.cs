using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using ProtobufDecoder.Application.Wpf.Commands;
using ProtobufDecoder.Application.Wpf.Models;

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
                Model.Message = ProtobufParser.Parse(bytes);
                Model.StatusBarInfo("File loaded successfully");
            }
            catch (Exception e)
            {
                Model.StatusBarError($"Failed to load file: {e.Message}");
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
                    Filter = "Protobuf files (.proto)|*.proto"
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
                Model.StatusBarInfo("Proto file saved successfully");
            }
            catch (Exception e)
            {
                Model.StatusBarError($"Failed to save file: {e.Message}");
            }
        }

        private void SaveGeneratedProtoFileAs()
        {
            var dialog = new SaveFileDialog
            {
                RestoreDirectory = true,
                AddExtension = true,
                DefaultExt = ".proto",
                Filter = "Protobuf files (.proto)|*.proto"
            };

            var result = dialog.ShowDialog();

            if (!result.HasValue || !result.Value)
            {
                return;
            }

            try
            {
                File.WriteAllText(dialog.FileName, ProtobufWriter.ToString(Model.Message));
                Model.StatusBarInfo($"Proto file saved successfully as {dialog.FileName}");
            }
            catch (Exception e)
            {
                Model.StatusBarError($"Failed to save file: {e.Message}");
            }
        }

        private void CopyTagValueToCsharpArray(ProtobufTag selectedTag)
        {
            if (selectedTag is ProtobufTagSingle singleTag and not ProtobufTagEmbeddedMessage)
            {
                var bytes = string.Join(", ", singleTag.Value.RawValue.Select(x => "0x" + x.ToString("X2").ToLower()));
                var csharpArray = $"var tagValueBytes = new byte[] {{{bytes}}};";

                Clipboard.SetText(csharpArray);

                Model.StatusBarInfo("Tag value copied to clipboard");
            }
        }

        private void DecodeTag(ProtobufTag selectedTag)
        {
            if (selectedTag is ProtobufTagSingle singleTag and not ProtobufTagEmbeddedMessage)
            {
                if (singleTag.Value.CanDecode)
                {
                    Model.DecodeTag(selectedTag);
                    Model.StatusBarInfo("Tag decoded successfully");
                    return;
                }
            }

            Model.StatusBarWarning("Cannot decode selected tag");
        }
    }
}