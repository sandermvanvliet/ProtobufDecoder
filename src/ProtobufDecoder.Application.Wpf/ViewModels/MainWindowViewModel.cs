using System.IO;
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
            LoadFileCommand = new RelayCommand(
                _ => LoadAndDecode(Model.InputFilePath),
                x => !string.IsNullOrEmpty(Model.InputFilePath));

            RenderProtoFileCommand = new RelayCommand(
                _ => RenderProtoFile(Model.Message),
                _ => Model.Message != null);

            OpenFileCommand = new RelayCommand(
                _ => OpenFile(),
                _ => true);

            SaveGeneratedProtoCommand = new RelayCommand(
                _ => SaveGeneratedProtoFile(Model.RenderedProtoFile),
                _ => !string.IsNullOrEmpty(Model.RenderedProtoFile));
        }

        public ICommand LoadFileCommand { get; }
        public ICommand OpenFileCommand { get; set; }
        public ICommand RenderProtoFileCommand { get; }
        public ICommand SaveGeneratedProtoCommand { get; }

        public MainWindowModel Model { get; set; }

        private void LoadAndDecode(string inputFilePath)
        {
            var bytes = File.ReadAllBytes(inputFilePath);
            Model.InputFileByteStream = new MemoryStream(bytes);
            Model.Message = ProtobufParser.Parse(bytes);
        }

        private void RenderProtoFile(ProtobufMessage modelMessage)
        {
            if (modelMessage.Name == null)
            {
                modelMessage.Name = "TestMessage";
            }

            Model.RenderedProtoFile = ProtobufWriter.ToString(modelMessage);
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

            if(result.HasValue && result.Value)
            {
                Model.InputFilePath = dialog.FileName;
            }
        }

        private void SaveGeneratedProtoFile(string renederedProtoFile)
        {
            var dialog = new SaveFileDialog
            {
                RestoreDirectory = true,
                AddExtension = true,
                DefaultExt = ".proto",
                Filter = "Protobuf files (.proto)|*.proto"
            };


            var result = dialog.ShowDialog();

            if(result.HasValue && result.Value)
            {
                File.WriteAllText(dialog.FileName, renederedProtoFile);
            }
        }
    }
}