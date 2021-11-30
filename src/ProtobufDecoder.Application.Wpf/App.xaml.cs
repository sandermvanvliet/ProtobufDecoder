using System.Linq;
using System.Windows;
using ProtobufDecoder.Application.Wpf.Models;
using ProtobufDecoder.Application.Wpf.ViewModels;

namespace ProtobufDecoder.Application.Wpf
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var viewModel = new MainWindowViewModel
            {
                Model = new MainWindowModel()
            };

            if (e.Args.Any())
            {
                viewModel.Model.InputFilePath = e.Args.First();
            }

            var mainWindow = new MainWindow(viewModel);

            mainWindow.Show();
        }
    }
}