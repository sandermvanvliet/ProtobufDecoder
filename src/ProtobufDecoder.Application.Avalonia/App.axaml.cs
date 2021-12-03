using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ProtobufDecoder.Application.Avalonia.ViewModels;
using ProtobufDecoder.Application.Avalonia.Views;

namespace ProtobufDecoder.Application.Avalonia
{
    public class App : global::Avalonia.Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindowViewModel = new MainWindowViewModel();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainWindowViewModel
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
