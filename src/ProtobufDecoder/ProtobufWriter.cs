using System;
using System.Linq;
using System.Text;
using Google.Protobuf;
using ProtobufDecoder.Tags;

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
                    RenderEmbeddedMessage(embeddedMessage, builder, tag, "    ");
                }
                else if(tag is ProtobufTagRepeated repeatedTag && repeatedTag.Items.Any(t => t is ProtobufTagEmbeddedMessage))
                {
                    var messageInstances = repeatedTag.Items.OfType<ProtobufTagEmbeddedMessage>().ToList();
                    // TODO: compare tag types between embedded messages
                    var groupedTags = messageInstances
                        .SelectMany(e => e.Tags)
                        .GroupBy(
                            t => t.Index,
                            t => t,
                            (key, values) =>
                            {
                                if (values.Count() == 1 && values.First() is ProtobufTagEmbeddedMessage)
                                {
                                    return values.First();
                                }

                                var firstTag = values.First();
                                return new ProtobufTagSingle
                                {
                                    Index = key,
                                    Name = firstTag.Name,
                                    WireType = firstTag.WireType,
                                    IsOptional = values.Count() != messageInstances.Count // Tag is required if it appears in all message instances
                                };
                            })
                        .ToArray();

                    // Create new tag instance to prevent nuking the original
                    var t = new ProtobufTagEmbeddedMessage(messageInstances.First(), groupedTags)
                    {
                        Name = messageInstances.First().Name
                    };

                    RenderEmbeddedMessage(t, builder, tag, "    ", true);
                }
                else if (tag is ProtobufTagPacked packedTag)
                {
                    var name = string.IsNullOrEmpty(packedTag.Name) ? "tag" + packedTag.Index : packedTag.Name;

                    builder.AppendLine($"    repeated {FormatWireTypeForProto(packedTag)} {name} = {packedTag.Index} [packed=true];");
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

        private static void RenderEmbeddedMessage(
            ProtobufTagEmbeddedMessage embeddedMessage, 
            StringBuilder builder,
            ProtobufTag tag,
            string indent,
            bool isRepeated = false)
        {
            var name = "tag" + embeddedMessage.Index;

            builder.AppendLine($"{indent}message {embeddedMessage.Name}");
            builder.AppendLine($"{indent}{{");
            foreach (var embeddedTag in embeddedMessage.Tags)
            {
                if (embeddedTag is ProtobufTagEmbeddedMessage nestedMessageTag)
                {
                    RenderEmbeddedMessage(nestedMessageTag, builder, embeddedTag, indent + "    ");
                }
                else
                {
                    var embeddedTagName = string.IsNullOrEmpty(embeddedTag.Name)
                        ? "tag" + embeddedTag.Index
                        : embeddedTag.Name;

                    builder.AppendLine(
                        $"{indent}    {(embeddedTag.IsOptional ? "optional " : "")}{FormatWireTypeForProto(embeddedTag)} {embeddedTagName} = {embeddedTag.Index};");
                }
            }

            builder.AppendLine($"{indent}}}");
            builder.AppendLine();

            builder.AppendLine($"{indent}{(isRepeated ? "repeated ": "")}{embeddedMessage.Name} {name} = {tag.Index};");
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
                case WireFormat.WireType.LengthDelimited when tag is ProtobufTagString:
                    type = "string";
                    break;
                case WireFormat.WireType.LengthDelimited:
                    type = "bytes";
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