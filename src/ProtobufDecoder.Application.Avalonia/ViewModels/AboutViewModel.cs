using ProtobufDecoder.Application.Avalonia.Models;

namespace ProtobufDecoder.Application.Avalonia.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public AboutViewModel()
        {
            Model = new AboutModel();
        }

        public AboutModel Model { get; set; }
    }
}