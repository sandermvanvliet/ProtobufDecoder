using System.Windows;
using System.Windows.Controls;

namespace ProtobufDecoder.Application.Wpf
{
    public class PropertyGridViewTemplateSelector : DataTemplateSelector
    {
        public DataTemplate String { get; set; }
        public DataTemplate Boolean { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ProtobufTagPropertyDescriptor descriptor)
            {
                if (descriptor.PropertyType == typeof(bool))
                {
                    return Boolean;
                }
            }

            return String;
        }
    }
}