using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ProtobufDecoder.Application.Wpf
{
    [ValueConversion(typeof(ProtobufTag), typeof(List<KeyValuePair<string, string>>))]
    public class ProtobufTagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tag = value as ProtobufTag;

            var properties = TypeDescriptor.GetProperties(tag);
            var list = new List<KeyValuePair<string, string>>();

            foreach (PropertyDescriptor p in properties)
            {
                list.Add(new KeyValuePair<string, string>(p.Name, p.GetValue(tag)?.ToString()));
            }

            return list;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}