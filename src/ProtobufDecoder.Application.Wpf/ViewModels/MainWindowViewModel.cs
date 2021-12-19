using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using ProtobufDecoder.Application.Wpf.Annotations;
using ProtobufDecoder.Application.Wpf.Commands;
using ProtobufDecoder.Application.Wpf.Models;

namespace ProtobufDecoder.Application.Wpf.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private MessageViewModel _message;

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
                _ =>
                {
                    if (((_ as TreeView)?.SelectedItem as ProtobufTagViewModel)?.CopyTagValueToCsharpArray() ?? false)
                    {
                        Model.StatusBarInfo(Strings.ContextMenuCopyValue);
                    }
                },
                _ => (_ as TreeView)?.SelectedItem is ProtobufTagViewModel);

            DecodeTagCommand = new RelayCommand(
                _ =>
                {
                    var parseResult = ((_ as TreeView)?.SelectedItem as ProtobufTagViewModel)?.DecodeTag();
                    if (parseResult == null)
                    {
                        Model.StatusBarWarning(Strings.CannotDecodeTag);
                    }
                    else if (parseResult.Success)
                    {
                        Model.StatusBarInfo(Strings.TagDecodedSuccessfully);
                    }
                    else
                    {
                        Model.StatusBarError(Strings.FailedToDecodeTag, parseResult.FailureReason);
                    }
                },
                _ => (_ as TreeView)?.SelectedItem is ProtobufTagViewModel viewModel && viewModel.CanDecode);
        }

        public ICommand LoadFileCommand { get; }
        public ICommand OpenFileCommand { get; set; }
        public ICommand SaveGeneratedProtoCommand { get; }
        public ICommand SaveGeneratedProtoAsCommand { get; }
        public ICommand CopyTagValueCommand { get; set; }
        public ICommand DecodeTagCommand { get; set; }

        public MainWindowModel Model { get; set; }

        public MessageViewModel Message
        {
            get => _message;
            set
            {
                if (Equals(value, _message)) return;
                _message = value;
                OnPropertyChanged();
            }
        }

        private void LoadAndDecode(string inputFilePath)
        {
            try
            {
                var bytes = File.ReadAllBytes(inputFilePath);
                Model.InputFileByteStream = new MemoryStream(bytes);
                var parseResult = ProtobufParser.Parse(bytes);

                if (parseResult.Success)
                {
                    Message = new MessageViewModel(parseResult.Message);
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}