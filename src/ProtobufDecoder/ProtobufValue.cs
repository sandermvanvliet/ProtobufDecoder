using System.ComponentModel;

namespace ProtobufDecoder
{
    public abstract class ProtobufValue
    {
        [Browsable(false)]
        public byte[] RawValue { get; set; }
    }

    public abstract class ProtobufValue<TValue> : ProtobufValue
    {
        [Browsable(true)]
        [ReadOnly(true)]
        [Description("The underlying value")]
        public TValue Value { get; }

        protected ProtobufValue(TValue value)
        {
            Value = value;
        }
    }
}
