using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ProtobufDecoder.Values;

namespace ProtobufDecoder
{
    public class ProtobufValueCollection : CollectionBase, ICustomTypeDescriptor
    {
        public ProtobufValueCollection(IEnumerable<ProtobufValue> values)
        {
            foreach (var x in values)
            {
                if (x != null)
                {
                    List.Add(x);
                }
            }
        }

        public ProtobufValue this[int index] => (ProtobufValue)List[index];

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public PropertyDescriptorCollection GetProperties()
        {
            var pds = new PropertyDescriptorCollection(null);

            for (var i = 0; i < List.Count; i++)
            {
                pds.Add(new ProtobufValueCollectionPropertyDescriptor(this, i));
            }

            return pds;
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }
    }
}