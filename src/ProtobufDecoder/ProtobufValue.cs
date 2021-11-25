namespace ProtobufDecoder
{
    public abstract class ProtobufValue
    {
    }

    public abstract class ProtobufValue<TValue> : ProtobufValue
    {
        public TValue Value { get; }

        protected ProtobufValue(TValue value)
        {
            Value = value;
        }
    }
}
