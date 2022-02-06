using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using ProtobufDecoder.Application.Wpf.Annotations;
using ProtobufDecoder.Application.Wpf.Commands;
using ProtobufDecoder.Application.Wpf.Models;
using ProtobufDecoder.Output;
using ProtobufDecoder.Output.Protobuf;

namespace ProtobufDecoder.Application.Wpf.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private MessageViewModel _message;
        private readonly IRenderer _renderer;

        public MainWindowViewModel()
        {
            _renderer = new Renderer();

            Model = new MainWindowModel();
            Message = new MessageViewModel();

            LoadFileCommand = new RelayCommand(
                _ => Message.LoadAndDecode(Model.InputFilePath),
                _ => !string.IsNullOrEmpty(Model?.InputFilePath))
                .OnSuccess(_ => Model.StatusBarInfo(Strings.FileLoadedSuccessfully))
                .OnSuccessWithWarnings(_ => Model.StatusBarInfo(Strings.FileLoadedWithWarnings, _.Message))
                .OnFailure(_ => Model.StatusBarError(Strings.FileFailedToLoad, _.Message));

            OpenFileCommand = new RelayCommand(
                _ => OpenFile(),
                _ => true);

            SaveGeneratedProtoCommand = new RelayCommand(
                _ => SaveGeneratedProtoFile(),
                _ => Message != null)
                .OnSuccess(_ => Model.StatusBarInfo(Strings.ProtoFileSavedSuccessfully))
                .OnFailure(_ => Model.StatusBarError(Strings.ProtoFileFailedToSave, _.Message));

            SaveGeneratedProtoAsCommand = new RelayCommand(
                _ => SaveGeneratedProtoFileAs(),
                _ => Message != null)
                .OnSuccess(_ => Model.StatusBarInfo(Strings.ProtoFileSavedAs, _.Message))
                .OnFailure(_ => Model.StatusBarError(Strings.ProtoFileFailedToSave, _.Message));

            CopyTagValueCommand = new RelayCommand(
                _ => ((_ as TreeView)?.SelectedItem as ProtobufTagViewModel)?.CopyTagValueToCsharpArray(),
                _ => (_ as TreeView)?.SelectedItem is ProtobufTagViewModel)
                .OnSuccess(_ => Model.StatusBarInfo(Strings.TagCopiedToClipboard));

            DecodeTagCommand = new RelayCommand(
                _ => ((_ as TreeView)?.SelectedItem as ProtobufTagViewModel)?.DecodeTag(),
                _ => (_ as TreeView)?.SelectedItem is ProtobufTagViewModel viewModel && viewModel.CanDecode)
                .OnSuccess(_ => Model.StatusBarInfo(Strings.TagDecodedSuccessfully))
                .OnSuccessWithWarnings(_ => Model.StatusBarWarning(Strings.CannotDecodeTag))
                .OnFailure(_ => Model.StatusBarError(Strings.FailedToDecodeTag, _.Message));

            DecodeAllTagsCommand = new RelayCommand(
                    _ => ((_ as TreeView)?.SelectedItem as ProtobufTagViewModel)?.DecodeTag(),
                    _ => (_ as TreeView)?.SelectedItem is ProtobufTagViewModel viewModel && viewModel.CanDecode)
                .OnSuccess(_ => Model.StatusBarInfo(Strings.TagDecodedSuccessfully))
                .OnSuccessWithWarnings(_ => Model.StatusBarWarning(Strings.CannotDecodeTag))
                .OnFailure(_ => Model.StatusBarError(Strings.FailedToDecodeTag, _.Message));

            LoadBytesFromClipboardCommand = new RelayCommand(
                _ => Message.LoadAndDecodeFromClipboard(),
                _ => Message != null)
                .OnSuccess(_ => Model.StatusBarInfo(Strings.FileLoadedSuccessfully))
                .OnSuccessWithWarnings(_ => Model.StatusBarInfo(Strings.FileLoadedWithWarnings, _.Message))
                .OnFailure(_ => Model.StatusBarError(Strings.FileFailedToLoad, _.Message));
        }

        public ICommand LoadBytesFromClipboardCommand { get; set; }
        public ICommand LoadFileCommand { get; }
        public ICommand OpenFileCommand { get; set; }
        public ICommand SaveGeneratedProtoCommand { get; }
        public ICommand SaveGeneratedProtoAsCommand { get; }
        public ICommand CopyTagValueCommand { get; set; }
        public ICommand DecodeTagCommand { get; set; }
        public ICommand DecodeAllTagsCommand { get; set; }

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

        private CommandResult OpenFile()
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

            return CommandResult.Success();
        }

        private CommandResult SaveGeneratedProtoFile()
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
                    return CommandResult.Success();
                }

                Model.OutputFilePath = dialog.FileName;
            }

            try
            {
                File.WriteAllText(Model.OutputFilePath, _renderer.Render(Message.Message));
                return CommandResult.Success();
            }
            catch (Exception e)
            {
                return CommandResult.Failure(e.Message);
            }
        }

        private CommandResult SaveGeneratedProtoFileAs()
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
                return CommandResult.Success();
            }

            try
            {
                File.WriteAllText(dialog.FileName, _renderer.Render(Message.Message));
                return CommandResult.Success(dialog.FileName);
            }
            catch (Exception e)
            {
                return CommandResult.Failure(e.Message);
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