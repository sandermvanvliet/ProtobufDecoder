using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ProtobufDecoder.Application.Avalonia.Converters
{
    public class ProtobufMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProtobufMessage message)
            {
                if (message.Name == null)
                {
                    message.Name = "TestMessage";
                }

                return ProtobufWriter.ToString(message);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}