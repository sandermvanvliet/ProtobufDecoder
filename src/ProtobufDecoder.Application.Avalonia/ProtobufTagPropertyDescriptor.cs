namespace ProtobufDecoder.Application.Avalonia
{
    public class ProtobufTagPropertyDescriptor
    {
        public ProtobufTagPropertyDescriptor(string name, string value, string category)
        {
            Name = name;
            Value = value;
            Category = category;
        }

        public string Name { get; }
        public string Value { get; }
        public string Category { get; }
    }
}