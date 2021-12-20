using System;
using System.Collections.Generic;
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
                    var protobufTags = tag
                        .Items
                        .OfType<ProtobufTagEmbeddedMessage>()
                        .SelectMany(t => t.Tags);

                    var groupedTags = AggregateEmbeddedMessageTags(protobufTags, tag.Items.Count);

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

        private static ProtobufTag[] AggregateEmbeddedMessageTags(IEnumerable<ProtobufTag> protobufTags, int itemsCount)
        {
            return protobufTags
                .GroupBy(
                    t => t.Index,
                    t => t,
                    (index, values) => AggregateTags(index, values.ToList(), itemsCount))
                .ToArray();
        }

        private static ProtobufTag AggregateTags(int index, List<ProtobufTag> tags, int itemCount)
        {
            var firstTag = tags.First();

            if (firstTag is ProtobufTagRepeated)
            {
                return new ProtobufTagRepeated
                {
                    Index = index,
                    WireType = firstTag.WireType,
                    IsOptional = tags.Count != itemCount
                };
            }

            if (firstTag is ProtobufTagPackedFloat)
            {
                return new ProtobufTagPackedFloat
                {
                    Index = index,
                    IsOptional = tags.Count != itemCount
                };
            }

            if (firstTag is ProtobufTagPackedVarint)
            {
                return new ProtobufTagPackedVarint
                {
                    Index = index,
                    IsOptional = tags.Count != itemCount
                };
            }

            if (firstTag is ProtobufTagString)
            {
                return new ProtobufTagString
                {
                    Index = index,
                    IsOptional = tags.Count != itemCount
                };
            }

            if (firstTag is ProtobufTagLengthDelimited)
            {
                return new ProtobufTagLengthDelimited
                {
                    Index = index,
                    IsOptional = tags.Count != itemCount
                };
            }

            if (firstTag is ProtobufTagEmbeddedMessage embedded)
            {
                var subTags = tags
                    .OfType<ProtobufTagEmbeddedMessage>()
                    .SelectMany(t => t.Tags)
                    .ToArray();

                    subTags = AggregateEmbeddedMessageTags(subTags, tags.Count);
                
                return new ProtobufTagEmbeddedMessage(embedded, subTags)
                {
                    Index = index,
                    Name = firstTag.Name
                };
            }

            return new ProtobufTagSingle
            {
                Index = index,
                WireType = firstTag.WireType,
                IsOptional = tags.Count != itemCount
            };
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
                case WireFormat.WireType.Varint when tag is ProtobufTagPackedVarint packedVarint:
                    type = "uint32";
                    break;
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