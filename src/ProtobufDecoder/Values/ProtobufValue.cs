using System.ComponentModel;

namespace ProtobufDecoder.Values
{
    public abstract class ProtobufValue
    {
        [Browsable(false)]
        public byte[] RawValue { get; set; }

        [Browsable(false)] public abstract bool CanDecode { get; }
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
