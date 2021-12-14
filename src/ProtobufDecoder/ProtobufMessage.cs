using System.Collections.Generic;
using ProtobufDecoder.Tags;

namespace ProtobufDecoder
{
    public class ProtobufMessage
    {
        public string Name { get; set; }
        public List<ProtobufTag> Tags { get; } = new List<ProtobufTag>();
    }
}