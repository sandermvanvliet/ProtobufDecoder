using System.Windows;
using System.Windows.Controls;
using ProtobufDecoder.Values;

namespace ProtobufDecoder.Application.Wpf
{
    public class PropertyGridViewTemplateSelector : DataTemplateSelector
    {
        public DataTemplate String { get; set; }
        public DataTemplate Boolean { get; set; }
        public DataTemplate PackedFloat { get; set; }
        public DataTemplate PackedVarint { get; set; }
        public DataTemplate Varint { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ProtobufTagPropertyDescriptor descriptor)
            {
                if (descriptor.PropertyType == typeof(bool))
                {
                    return Boolean;
                }

                if (descriptor.PropertyType == typeof(ProtobufValue) && descriptor.Value != null)
                {
                    var valueType = descriptor.Value.GetType();

                    if (valueType == typeof(PackedFloatValue))
                    {
                        return PackedFloat;
                    }

                    if (valueType == typeof(PackedVarintValue))
                    {
                        return PackedVarint;
                    }

                    if (valueType == typeof(VarintValue))
                    {
                        return Varint;
                    }
                }
            }

            return String;
        }
    }
}