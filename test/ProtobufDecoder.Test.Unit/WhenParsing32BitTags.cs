using System.Linq;
using FluentAssertions;
using Google.Protobuf;
using ProtobufDecoder.Tags;
using ProtobufDecoder.Values;
using Xunit;

namespace ProtobufDecoder.Test.Unit
{
    
    public class WhenParsing32BitTags
    {
        [Fact]
        public void SingleFixed32_TagNumberIsOne()
        {
            var input = new byte[] { 0x0d, 0x4B, 0x06, 0x9E, 0x3F };

            var parseResult = ProtobufParser.Parse(input);

            parseResult
				.Message
                .Tags
                .Should()
                .OnlyContain(t => t.Index == 1);
        }

        [Fact]
        public void SingleFixed32_WireTypeIsFixed32()
        {
            var input = new byte[] { 0x0d, 0x4B, 0x06, 0x9E, 0x3F };

            var parseResult = ProtobufParser.Parse(input);

            parseResult
				.Message
                .Tags
                .Should()
                .OnlyContain(t => t.WireType == WireFormat.WireType.Fixed32);
        }

        [Fact]
        public void SingleFixed32_ValueIsOfTypeFixed32()
        {
            var input = new byte[] { 0x0d, 0x4B, 0x06, 0x9E, 0x3F };

            var parseResult = ProtobufParser.Parse(input);

            parseResult
				.Message
                .Tags
                .OfType<ProtobufTagSingle>()
                .Should()
                .OnlyContain(t => t.Value is Fixed32Value);
        }

        [Fact]
        public void SingleFixed32_ValueIsOfTypeFixed32WithValue()
        {
            var input = new byte[] { 0x0d, 0x4B, 0x06, 0x9E, 0x3F };

            var parseResult = ProtobufParser.Parse(input);

            parseResult
				.Message
                .Tags
                .OfType<ProtobufTagSingle>()
                .Single()
                .Value
                .As<Fixed32Value>()
                .Value
                .Should()
                .Be((float)1.234567);
        }

        [Fact]
        public void TwoVarintTags_TwoTagsAreFound()
        {
            var input = new byte[] { 0x0d, 0x4B, 0x06, 0x9E, 0x3F, 0x15, 0x4B, 0x06, 0x9E, 0x3F };

            var parseResult = ProtobufParser.Parse(input);

            parseResult
				.Message
                .Tags
                .Should()
                .Contain(t => t.Index == 1 && t.WireType == WireFormat.WireType.Fixed32)
                .And
                .Contain(t => t.Index == 2 && t.WireType == WireFormat.WireType.Fixed32);
        }

        [Fact]
        public void TwoVarintTags_TagTwoIsFixed32WithValue()
        {
            var input = new byte[] { 0x0d, 0x4B, 0x06, 0x9E, 0x3F, 0x15, 0x4B, 0x06, 0x9E, 0x3F };

            var parseResult = ProtobufParser.Parse(input);

            parseResult
				.Message
                .Tags
                .OfType<ProtobufTagSingle>()
                .Single(t => t.Index == 2)
                .Value
                .As<Fixed32Value>()
                .Value
                .Should()
                .Be((float)1.234567);
        }
    }
}