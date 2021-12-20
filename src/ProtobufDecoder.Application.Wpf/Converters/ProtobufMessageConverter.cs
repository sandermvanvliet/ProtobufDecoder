using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ProtobufDecoder.Output.Protobuf;

namespace ProtobufDecoder.Application.Wpf.Converters
{
    [ValueConversion(typeof(ProtobufMessage), typeof(string))]
    public class ProtobufMessageConverter : IValueConverter
    {
        private readonly Renderer _renderer;

        public ProtobufMessageConverter()
        {
            _renderer = new Renderer();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProtobufMessage message)
            {
                if (message.Name == null)
                {
                    message.Name = "TestMessage";
                }

                return _renderer.Render(message);
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}