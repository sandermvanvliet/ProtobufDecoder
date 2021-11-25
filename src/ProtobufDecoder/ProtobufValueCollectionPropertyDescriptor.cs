using System;
using System.ComponentModel;

namespace ProtobufDecoder
{
    public class ProtobufValueCollectionPropertyDescriptor : PropertyDescriptor
    {
        private readonly ProtobufValueCollection _collection;
        private readonly int _index = -1;

        public ProtobufValueCollectionPropertyDescriptor(ProtobufValueCollection coll, int idx) : base("#" + idx, null)
        {
            _collection = coll;
            _index = idx;
        }

        public override AttributeCollection Attributes => new(null);

        public override Type ComponentType => _collection.GetType();

        public override string DisplayName
        {
            get
            {
                var val = _collection[_index];

                return val.GetType().Name;
            }
        }

        public override string Description
        {
            get
            {
                var emp = _collection[_index];

                return emp.GetType().Name);
            }
        }

        public override bool IsReadOnly => true;

        public override string Name => "#" + _index;

        public override Type PropertyType => _collection[_index].GetType();

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override object GetValue(object component)
        {
            return _collection[_index];
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        public override void SetValue(object component, object value)
        {
        }
    }
}