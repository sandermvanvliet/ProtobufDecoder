using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ProtobufDecoder.Application.Wpf.Annotations;

namespace ProtobufDecoder.Application.Wpf.Converters
{
    public class ProtobufTagPropertyDescriptor : INotifyPropertyChanged
    {
        private readonly PropertyDescriptor _propertyDescriptor;
        private readonly object _instance;
        private object _value;

        public ProtobufTagPropertyDescriptor(PropertyDescriptor propertyDescriptor,
            object instance,
            string category, 
            bool isReadOnly)
        {
            _propertyDescriptor = propertyDescriptor;
            _instance = instance;
            Name = propertyDescriptor.DisplayName;
            Value = propertyDescriptor.GetValue(instance);
            PropertyType = propertyDescriptor.PropertyType;
            Category = category;
            IsReadOnly = isReadOnly;
            Description = propertyDescriptor.Attributes.OfType<DescriptionAttribute>().SingleOrDefault()?.Description;
        }

        public string Name { get; }

        public object Value
        {
            get => _value;
            set
            {
                if (Equals(value, _value))
                {
                    return;
                }

                _value = value;
                
                _propertyDescriptor.SetValue(_instance, _value);

                OnPropertyChanged();
            }
        }

        public Type PropertyType { get; set; }
        public string Category { get; }
        public bool IsReadOnly { get; }
        public string Description { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}