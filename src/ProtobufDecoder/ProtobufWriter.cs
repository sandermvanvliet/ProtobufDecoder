using System;
using System.Text;
using Google.Protobuf;

namespace ProtobufDecoder
{
    public class ProtobufWriter
    {
        public static string ToString(ProtobufMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (string.IsNullOrEmpty(message.Name))
            {
                throw new ArgumentException("Message name cannot be empty");
            }

            var builder = new StringBuilder();

            builder.AppendLine($"message {message.Name}");
            builder.AppendLine("{");
            
            foreach (var tag in message.Tags)
            {
                if (tag is ProtobufTagEmbeddedMessage embeddedMessage)
                {
                    var name = "tag" + embeddedMessage.Index;

                    builder.AppendLine($"    message {embeddedMessage.Name}");
                    builder.AppendLine("    {");
                    foreach (var embeddedTag in embeddedMessage.Tags)
                    {
                        var embeddedTagName = string.IsNullOrEmpty(embeddedTag.Name) ? "tag" + embeddedTag.Index : embeddedTag.Name;

                        builder.AppendLine($"        {FormatWireTypeForProto(embeddedTag)} {embeddedTagName} = {embeddedTag.Index};");
                    }
                    builder.AppendLine("    }");
                    builder.AppendLine();

                    builder.AppendLine($"    {embeddedMessage.Name} {name} = {tag.Index};");
                }

                else
                {
                    var name = string.IsNullOrEmpty(tag.Name) ? "tag" + tag.Index : tag.Name;

                    builder.AppendLine($"    {FormatWireTypeForProto(tag)} {name} = {tag.Index};");
                }
            }

            builder.AppendLine("}");

            return builder.ToString();
        }

        private static string FormatWireTypeForProto(ProtobufTag tag)
        {
            string type;

            switch (tag.WireType)
            {
                case WireFormat.WireType.Varint:
                    type = "uint32";
                    break;
                case WireFormat.WireType.Fixed64:
                    type = "double";
                    break;
                case WireFormat.WireType.LengthDelimited:
                    type = "string"; // Whatevs
                    break;
                case WireFormat.WireType.StartGroup:
                case WireFormat.WireType.EndGroup:
                    type = "group";
                    break;
                case WireFormat.WireType.Fixed32:
                    type = "float";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unknown wire type " + (int)tag.WireType);
            }

            if (tag is ProtobufTagRepeated)
            {
                type = "repeated " + type;
            }

            return type;
        }
    }
}