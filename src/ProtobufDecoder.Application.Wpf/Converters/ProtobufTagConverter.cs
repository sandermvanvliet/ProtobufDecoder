using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ProtobufDecoder.Application.Wpf.ViewModels;
using ProtobufDecoder.Tags;

namespace ProtobufDecoder.Application.Wpf.Converters
{
    [ValueConversion(typeof(ProtobufTag), typeof(List<ProtobufTagPropertyDescriptor>))]
    public class ProtobufTagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var list = new List<ProtobufTagPropertyDescriptor>();

            if (value is ProtobufTagViewModel viewModel)
            {
                // Ensure that the type of the tag always appears in the list of properties
                list.Add(new ProtobufTagPropertyDescriptor(
                    new ManualPropertyDescriptor("Type", viewModel.Tag.GetType().Name),
                    viewModel.Tag,
                    null,
                    true));

                var properties = TypeDescriptor.GetProperties(viewModel.Tag);

                foreach (PropertyDescriptor p in properties)
                {
                    if (HasAttribute<BrowsableAttribute>(p.Attributes,a => a.Browsable == false))
                    {
                        continue;
                    }

                    list.Add(new ProtobufTagPropertyDescriptor(
                        p, 
                        viewModel.Tag, 
                        GetCategoryOf(p), 
                        HasAttribute<ReadOnlyAttribute>(p.Attributes, a => a.IsReadOnly)));
                }
            }

            return list
                .OrderBy(d => d.Category)
                .ThenBy(d => d.Name)
                .ToList();
        }

        private static string GetCategoryOf(PropertyDescriptor propertyDescriptor)
        {
            return propertyDescriptor
                .Attributes
                .OfType<CategoryAttribute>()
                .SingleOrDefault()?
                .Category;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        private static bool HasAttribute<TAttribute>(
            AttributeCollection attributes,
            Func<TAttribute, bool> predicate)
        {
            return attributes.OfType<TAttribute>().Any(predicate);
        }
    }
}