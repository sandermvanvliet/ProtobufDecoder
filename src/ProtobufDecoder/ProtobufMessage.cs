using System.Collections.Generic;

namespace ProtobufDecoder
{
    public class ProtobufMessage
    {
        public string Name { get; set; }
        public List<ProtobufTag> Tags { get; } = new List<ProtobufTag>();
    }
}