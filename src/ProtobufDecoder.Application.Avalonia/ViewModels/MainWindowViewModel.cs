using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ProtobufDecoder.Application.Avalonia.Commands;
using ProtobufDecoder.Application.Avalonia.Models;

namespace ProtobufDecoder.Application.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
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
                Model.StatusBarInfo(Strings.FileLoadedSuccessfully);
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
                AllowMultiple = false
            };

            var result = dialog.ShowAsync(GetMainWindow()).GetAwaiter().GetResult();

            if(result != null && result.Any())
            {
                Model.InputFilePath = result.First();

                LoadFileCommand.Execute(null);
            }
        }

        private void SaveGeneratedProtoFile()
        {
            if (string.IsNullOrEmpty(Model.OutputFilePath))
            {
                var dialog = new SaveFileDialog();
                
                dialog.DefaultExtension = ".proto";
                dialog.Filters.Add(new FileDialogFilter{ Extensions = new List<string>{".proto"}, Name = Strings.ProtoFileType});

                var result = dialog.ShowAsync(GetMainWindow()).GetAwaiter().GetResult();

                if (!string.IsNullOrEmpty(result))
                {
                    Model.OutputFilePath = result;
                }
                else
                {
                    return;
                }
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
            var dialog = new SaveFileDialog();
                
            dialog.DefaultExtension = ".proto";
            dialog.Filters.Add(new FileDialogFilter{ Extensions = new List<string>{".proto"}, Name = Strings.ProtoFileType});

            var result = dialog.ShowAsync(GetMainWindow()).GetAwaiter().GetResult();

            if (!string.IsNullOrEmpty(result))
            {
                try
                {
                    File.WriteAllText(result, ProtobufWriter.ToString(Model.Message));
                    Model.StatusBarInfo(Strings.ProtoFileSavedAs, result);
                }
                catch (Exception e)
                {
                    Model.StatusBarError(Strings.ProtoFileFailedToSave, e.Message);
                }
            }
        }

        private void CopyTagValueToCsharpArray(ProtobufTag selectedTag)
        {
            if (selectedTag is ProtobufTagSingle singleTag and not ProtobufTagEmbeddedMessage)
            {
                var bytes = string.Join(", ", singleTag.Value.RawValue.Select(x => "0x" + x.ToString("X2").ToLower()));
                var csharpArray = $"var tagValueBytes = new byte[] {{{bytes}}};";
                
                global::Avalonia.Application.Current.Clipboard.SetTextAsync(csharpArray)
                    .GetAwaiter()
                    .GetResult();

                Model.StatusBarInfo(Strings.TagCopiedToClipboard);
            }
        }

        private void DecodeTag(ProtobufTag selectedTag)
        {
            if (selectedTag is ProtobufTagSingle singleTag and not ProtobufTagEmbeddedMessage)
            {
                if (singleTag.Value.CanDecode)
                {
                    Model.DecodeTag(selectedTag);
                    Model.StatusBarInfo(Strings.TagDecodedSuccessfully);
                    return;
                }
            }

            Model.StatusBarWarning(Strings.CannotDecodeTag);
        }

        private static Window GetMainWindow()
        {
            if (global::Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow;
            }

            return null;
        }
    }
}