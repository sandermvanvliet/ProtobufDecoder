using System;
using System.ComponentModel;
using ProtobufDecoder.Tags;

namespace ProtobufDecoder.Application.Wpf.Converters
{
    /// <summary>
    /// A property descriptor that is initialized manually instead of through a TypeDescriptor
    /// </summary>
    public class ManualPropertyDescriptor : PropertyDescriptor
    {
        private readonly string _value;

        public ManualPropertyDescriptor(string name, string value) : base(name, Array.Empty<Attribute>())
        {
            _value = value;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            return _value;
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override Type ComponentType => typeof(ProtobufTag);
        public override bool IsReadOnly => true;
        public override Type PropertyType => typeof(string);
    }
}