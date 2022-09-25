﻿#if SCREEN_RECORDING
using System;
using System.Reflection;
#endif
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
#if SCREEN_RECORDING
            var menuDropAlignmentField = typeof(SystemParameters).GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);

            Action setAlignmentValue = () => 
            {
                if (SystemParameters.MenuDropAlignment && menuDropAlignmentField != null)
                {
                    menuDropAlignmentField.SetValue(null, false);
                }
            };

            setAlignmentValue();
            
            SystemParameters.StaticPropertyChanged += (sender, e) => { setAlignmentValue(); };
#endif

            //Force program language (test specific)
            //The program automatically judges according to the system language by default
            //Strings.Culture = new System.Globalization.CultureInfo("zh-CN");
            //Strings.Culture = new System.Globalization.CultureInfo("en-US");
            //Strings.Culture = new System.Globalization.CultureInfo("nl-NL");

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