using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ProtobufDecoder.Application.Wpf
{
    [ValueConversion(typeof(ProtobufTag), typeof(List<ProtobufTagPropertyDescriptor>))]
    public class ProtobufTagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tag = value as ProtobufTag;

            var properties = TypeDescriptor.GetProperties(tag);
            var list = new List<ProtobufTagPropertyDescriptor>();

            foreach (PropertyDescriptor p in properties)
            {
                if(p.Attributes.OfType<BrowsableAttribute>().Any(a => a.Browsable == false))
                {
                    continue;
                }

                var category = GetCategoryOf(p);

                list.Add(new ProtobufTagPropertyDescriptor(p.Name, p.GetValue(tag)?.ToString(), category));
            }

            return list;
        }

        private static string GetCategoryOf(PropertyDescriptor propertyDescriptor)
        {
            var category = propertyDescriptor.Attributes.OfType<CategoryAttribute>().SingleOrDefault();

            return category?.Category;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}