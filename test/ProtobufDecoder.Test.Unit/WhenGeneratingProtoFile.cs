using FluentAssertions;
using Google.Protobuf;
using Xunit;

namespace ProtobufDecoder.Test.Unit
{
    public class WhenGeneratingProtoFile
    {
        [Fact]
        public void GivenMessageWithoutTags_OnlyMessageIsWritten()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage"
            };

            var proto = ProtobufWriter.ToString(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
}
");
        }

        [Fact]
        public void GivenMessageWithSingleVarintTag()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                    new ProtobufTagSingle
                    {
                        Index = 1,
                        WireType = WireFormat.WireType.Varint
                    }
                }
            };

            var proto = ProtobufWriter.ToString(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    uint32 tag1 = 1;
}
");
        }

        [Fact]
        public void GivenMessageWithSingleString()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                    new ProtobufTagSingle
                    {
                        Index = 1,
                        WireType = WireFormat.WireType.LengthDelimited
                    }
                }
            };

            var proto = ProtobufWriter.ToString(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    string tag1 = 1;
}
");
        }

        [Fact]
        public void GivenMessageWithSingleFixed32()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                    new ProtobufTagSingle
                    {
                        Index = 1,
                        WireType = WireFormat.WireType.Fixed32
                    }
                }
            };

            var proto = ProtobufWriter.ToString(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    float tag1 = 1;
}
");
        }

        [Fact]
        public void GivenMessageWithSingleFixed64()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                    new ProtobufTagSingle
                    {
                        Index = 1,
                        WireType = WireFormat.WireType.Fixed64
                    }
                }
            };

            var proto = ProtobufWriter.ToString(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    double tag1 = 1;
}
");
        }

        [Fact]
        public void GivenMessageWithRepeatedFixed64()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                    new ProtobufTagRepeated
                    {
                        Index = 1,
                        WireType = WireFormat.WireType.Fixed64
                    }
                }
            };

            var proto = ProtobufWriter.ToString(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    repeated double tag1 = 1;
}
");
        }

        [Fact]
        public void GivenMessageWithSingleEmbeddedMessage()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                    new ProtobufTagEmbeddedMessage(new ProtobufTagSingle
                        {
                            Index = 10,
                            WireType = WireFormat.WireType.LengthDelimited
                        },
                        new ProtobufTag[]
                        {
                            new ProtobufTagSingle
                            {
                                Index = 1,
                                WireType = WireFormat.WireType.Fixed64
                            }
                        })
                    {
                        Name = "EmbeddedMessage"
                    }
                }
            };

            var proto = ProtobufWriter.ToString(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    message EmbeddedMessage
    {
        double tag1 = 1;
    }

    EmbeddedMessage tag10 = 10;
}
");
        }
    }
}
