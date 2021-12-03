using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ProtobufDecoder.Application.Avalonia.ViewModels;

namespace ProtobufDecoder.Application.Avalonia.Views
{
    public partial class About : Window
    {
        public About()
        {
            DataContext = new AboutViewModel();
            
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OkButton_OnClick(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
