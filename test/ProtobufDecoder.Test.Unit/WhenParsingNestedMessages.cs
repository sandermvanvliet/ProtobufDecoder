using FluentAssertions;
using Google.Protobuf;
using ProtobufDecoder.Tags;
using Xunit;

namespace ProtobufDecoder.Test.Unit
{
    public class WhenParsingNestedMessages
    {
        [Fact]
        public void GivenPayloadWithNestedMessages_ParsingFirstLevelTagsSucceeds()
        {
            var root = new Message1
            {
                IntValue = 1,
                StringValue = "one",
                ByteValue = ByteString.CopyFrom(0x1, 0x1),
                ObjValue = new Message2
                {
                    IntValue = 2,
                    StringValue = "two",
                    ByteValue = ByteString.CopyFrom(0x2, 0x2),
                    ObjValue = new Message3
                    {
                        IntValue = 3,
                        StringValue = "three",
                        ByteValue = ByteString.CopyFrom(0x3, 0x3),
                    }
                }
            };

            var input = root.ToByteArray();

            var parseResult = ProtobufParser.Parse(input);

            parseResult
                .Message
                .Tags
                .Should()
                .HaveCount(4);
        }

        [Fact]
        public void GivenPayloadWithNestedMessages_ParsingSecondLevelTagsSucceeds()
        {
            var root = new Message1
            {
                IntValue = 1,
                StringValue = "one",
                ByteValue = ByteString.CopyFrom(0x1, 0x1),
                ObjValue = new Message2
                {
                    IntValue = 2,
                    StringValue = "two",
                    ByteValue = ByteString.CopyFrom(0x2, 0x2),
                    ObjValue = new Message3
                    {
                        IntValue = 3,
                        StringValue = "three",
                        ByteValue = ByteString.CopyFrom(0x3, 0x3),
                    }
                }
            };

            var input = root.ToByteArray();

            var parseResult = ProtobufParser.Parse(input);
            parseResult = ProtobufParser.Parse((parseResult.Message.Tags[3] as ProtobufTagSingle).Value.RawValue);
            
            parseResult
                .Message
                .Tags
                .Should()
                .HaveCount(4);
        }

        [Fact]
        public void GivenPayloadWithNestedMessages_ParsingThirdLevelTagsSucceeds()
        {
            var root = new Message1
            {
                IntValue = 1,
                StringValue = "one",
                ByteValue = ByteString.CopyFrom(0x1, 0x1),
                ObjValue = new Message2
                {
                    IntValue = 2,
                    StringValue = "two",
                    ByteValue = ByteString.CopyFrom(0x2, 0x2),
                    ObjValue = new Message3
                    {
                        IntValue = 3,
                        StringValue = "three",
                        ByteValue = ByteString.CopyFrom(0x3, 0x3),
                    }
                }
            };

            var input = root.ToByteArray();

            var parseResult = ProtobufParser.Parse(input);
            parseResult = ProtobufParser.Parse((parseResult.Message.Tags[3] as ProtobufTagSingle).Value.RawValue);
            parseResult = ProtobufParser.Parse((parseResult.Message.Tags[3] as ProtobufTagSingle).Value.RawValue);
            
            parseResult
                .Message
                .Tags
                .Should()
                .HaveCount(3);
        }
    }
}