using System.Collections.Generic;

namespace ProtobufDecoder
{
    public class ProtobufMessage
    {
        public List<ProtobufTag> Tags { get; } = new List<ProtobufTag>();
    }
}