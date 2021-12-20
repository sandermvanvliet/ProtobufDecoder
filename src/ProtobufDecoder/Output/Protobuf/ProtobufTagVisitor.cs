using System;
using System.Linq;
using System.Text;
using Google.Protobuf;
using ProtobufDecoder.Tags;
using ProtobufDecoder.Values;

namespace ProtobufDecoder.Output.Protobuf
{
    public class ProtobufTagVisitor : ProtobufTagVisitorBase
    {
        public const string DefaultIndent = "    ";

        private string _indent;

        public ProtobufTagVisitor(StringBuilder builder, string indent = null) : base(builder)
        {
            _indent = indent ?? string.Empty;
        }

        protected override void Visit(ProtobufTagSingle tag)
        {
            var optionality = tag.IsOptional ? "optional " : string.Empty;
            var type = TypeOf(tag);

            Builder.AppendLine($"{_indent}{optionality}{type} {tag.Name} = {tag.Index};");
        }

        protected override void Visit(ProtobufTagRepeated tag)
        {
            if (tag.ContainsOnlyEmbeddedMessages)
            {
                var embeddedMessageTag = tag.Items.First() as ProtobufTagEmbeddedMessage;
                var optionality = tag.IsOptional ? "optional " : string.Empty;
                var type = tag.Items.First().Name;

                if (tag.Items.Count > 1)
                {
                    var groupedTags = tag
                        .Items
                        .OfType<ProtobufTagEmbeddedMessage>()
                        .SelectMany(t => t.Tags)
                        .GroupBy<ProtobufTag, int, ProtobufTag, ProtobufTag>(
                            t => t.Index,
                            t => t,
                            (index, values) =>
                            {
                                var tags = values.ToList();

                                return new ProtobufTagSingle
                                {
                                    Index = index,
                                    WireType = tags.First().WireType,
                                    IsOptional = tags.Count() != tag.Items.Count
                                };
                            })
                        .ToArray();

                    embeddedMessageTag = new ProtobufTagEmbeddedMessage(
                        embeddedMessageTag,
                        groupedTags)
                    {
                        Name = tag.Items.First().Name
                    };
                }

                DefinitionOfEmbeddedMessage(embeddedMessageTag);

                Builder.AppendLine();
                Builder.AppendLine($"{_indent}{optionality}repeated {type} {tag.Name} = {tag.Index};");
            }
            else
            {
                var optionality = tag.IsOptional ? "optional " : string.Empty;
                var type = TypeOf(tag);

                Builder.AppendLine($"{_indent}{optionality}repeated {type} {tag.Name} = {tag.Index};");
            }
        }

        protected override void Visit(ProtobufTagEmbeddedMessage tag)
        {
            DefinitionOfEmbeddedMessage(tag);
            Builder.AppendLine();
            Builder.AppendLine($"{_indent}{tag.Name} tag{tag.Index} = {tag.Index};");
        }

        private void DefinitionOfEmbeddedMessage(ProtobufTagEmbeddedMessage tag)
        {
            Builder.AppendLine($"{_indent}message {tag.Name}");
            Builder.AppendLine($"{_indent}{{");

            PushIndent();

            foreach (var childTag in tag.Tags)
            {
                Visit(childTag);
            }

            PopIndent();

            Builder.AppendLine($"{_indent}}}");
        }

        private void PushIndent()
        {
            _indent += DefaultIndent;
        }

        private void PopIndent()
        {
            _indent = _indent.Substring(0, _indent.Length - DefaultIndent.Length);
        }

        protected override void Visit(ProtobufTagPacked tag)
        {
            var optionality = tag.IsOptional ? "optional " : string.Empty;
            var type = TypeOf(tag);

            Builder.AppendLine($"{_indent}{optionality}repeated {type} {tag.Name} = {tag.Index} [packed=true];");
        }

        protected override void Visit(ProtobufTagString tag)
        {
            var optionality = tag.IsOptional ? "optional " : string.Empty;
            var type = "string";

            Builder.AppendLine($"{_indent}{optionality}{type} {tag.Name} = {tag.Index};");
        }

        private static string TypeOf(ProtobufTag tag)
        {
            string type;

            switch (tag.WireType)
            {
                case WireFormat.WireType.Varint when tag is ProtobufTagSingle singleTag:
                    type = ((VarintValue)singleTag.Value)?.GetProtobufType() ?? "uint32";
                    break;
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

            return type;
        }
    }
}