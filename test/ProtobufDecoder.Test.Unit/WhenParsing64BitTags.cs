using System.Linq;
using FluentAssertions;
using Google.Protobuf;
using Xunit;

namespace ProtobufDecoder.Test.Unit
{
    public class WhenParsing64BitTags
    {
        [Fact]
        public void SingleFixed64_TagNumberIsOne()
        {
            var input = new byte[] { 0x9, 0x49, 0x94, 0xF9, 0xA0 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .OnlyContain(t => t.Index == 1);
        }

        [Fact]
        public void SingleFixed64_WireTypeIsFixed64()
        {
            var input = new byte[] { 0x9, 0x49, 0x94, 0xF9, 0xA0 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .OnlyContain(t => t.WireType == WireFormat.WireType.Fixed64);
        }

        [Fact]
        public void SingleFixed64_ValueIsOfTypeFixed64()
        {
            var input = new byte[] { 0x9, 0x49, 0x94, 0xF9, 0xA0 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .OnlyContain(t => t.Value is Fixed64Value);
        }

        [Fact]
        public void SingleFixed64_ValueIsOfTypeFixed64WithValue()
        {
            var input = new byte[] { 0x9, 0x49, 0x94, 0xF9, 0xA0 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Single()
                .Value
                .As<Fixed64Value>()
                .Value
                .Should()
                .BeEquivalentTo(new[] { 0x49, 0x94, 0xF9, 0xA0 });
        }

        [Fact]
        public void TwoVarintTags_TwoTagsAreFound()
        {
            var input = new byte[] { 0x9, 0x49, 0x94, 0xF9, 0xA0, 0x11, 0x49, 0x94, 0xF9, 0xA0, 0x11 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .Contain(t => t.Index == 1 && t.WireType == WireFormat.WireType.Fixed64)
                .And
                .Contain(t => t.Index == 2 && t.WireType == WireFormat.WireType.Fixed64);
        }

        [Fact]
        public void TwoVarintTags_TagTwoIsFixed64WithValue()
        {
            var input = new byte[] { 0x9, 0x49, 0x94, 0xF9, 0xA0, 0x11, 0x49, 0x94, 0xF9, 0xA0 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Single(t => t.Index == 2)
                .Value
                .As<Fixed64Value>()
                .Value
                .Should()
                .BeEquivalentTo(new[] { 0x49, 0x94, 0xF9, 0xA0 });
        }
    }
}