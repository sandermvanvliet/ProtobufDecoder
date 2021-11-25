using System.Linq;
using FluentAssertions;
using Google.Protobuf;
using Xunit;

namespace ProtobufDecoder.Test.Unit
{
    
    public class WhenParsing32BitTags
    {
        [Fact]
        public void SingleFixed32_TagNumberIsOne()
        {
            var input = new byte[] { 0x0d, 0x49, 0x94 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .OnlyContain(t => t.Index == 1);
        }

        [Fact]
        public void SingleFixed32_WireTypeIsFixed32()
        {
            var input = new byte[] { 0x0d, 0x49, 0x94 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .OnlyContain(t => t.WireType == WireFormat.WireType.Fixed32);
        }

        [Fact]
        public void SingleFixed32_ValueIsOfTypeFixed32()
        {
            var input = new byte[] { 0x0d, 0x49, 0x94 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .OnlyContain(t => t.Value is Fixed32Value);
        }

        [Fact]
        public void SingleFixed32_ValueIsOfTypeFixed32WithValue()
        {
            var input = new byte[] { 0x0d, 0x49, 0x94 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Single()
                .Value
                .As<Fixed32Value>()
                .Value
                .Should()
                .BeEquivalentTo(new[] { 0x49, 0x94 });
        }

        [Fact]
        public void TwoVarintTags_TwoTagsAreFound()
        {
            var input = new byte[] { 0x0d, 0x49, 0x94, 0x15, 0x49, 0x94 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .Contain(t => t.Index == 1 && t.WireType == WireFormat.WireType.Fixed32)
                .And
                .Contain(t => t.Index == 2 && t.WireType == WireFormat.WireType.Fixed32);
        }

        [Fact]
        public void TwoVarintTags_TagTwoIsFixed32WithValue()
        {
            var input = new byte[] { 0x0d, 0x49, 0x94, 0x15, 0x49, 0x94 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Single(t => t.Index == 2)
                .Value
                .As<Fixed32Value>()
                .Value
                .Should()
                .BeEquivalentTo(new[] { 0x49, 0x94 });
        }
    }
}