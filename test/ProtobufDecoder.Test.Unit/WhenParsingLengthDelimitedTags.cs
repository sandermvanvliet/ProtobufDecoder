using System.Linq;
using FluentAssertions;
using Google.Protobuf;
using Xunit;

namespace ProtobufDecoder.Test.Unit
{
    
    public class WhenParsingLengthDelimitedTags
    {
        [Fact]
        public void SingleString_TagNumberIsOne()
        {
            var input = new byte[] { 0x0a, 0x07, 0x74, 0x65, 0x73, 0x74, 0x69, 0x6e, 0x67 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .OnlyContain(t => t.Index == 1);
        }

        [Fact]
        public void SingleString_WireTypeIsLengthDelimited()
        {
            var input = new byte[] { 0x0a, 0x07, 0x74, 0x65, 0x73, 0x74, 0x69, 0x6e, 0x67 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .OnlyContain(t => t.WireType == WireFormat.WireType.LengthDelimited);
        }

        [Fact]
        public void SingleString_ValueIsOfTypeLengthDelimited()
        {
            var input = new byte[] { 0x0a, 0x07, 0x74, 0x65, 0x73, 0x74, 0x69, 0x6e, 0x67 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .OfType<ProtobufTagSingle>()
                .Should()
                .OnlyContain(t => t.Value is LengthDelimitedValue);
        }

        [Fact]
        public void SingleString_ValueIsOfTypeLengthDelimitedWithValueTesting()
        {
            var input = new byte[] { 0x0a, 0x07, 0x74, 0x65, 0x73, 0x74, 0x69, 0x6e, 0x67 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .OfType<ProtobufTagSingle>()
                .Should()
                .OnlyContain(t => ((LengthDelimitedValue)t.Value).StringRepresentation == "testing");
        }

        [Fact]
        public void TwoVarintTags_TwoTagsAreFound()
        {
            var input = new byte[] { 0x0a, 0x07, 0x74, 0x65, 0x73, 0x74, 0x69, 0x6e, 0x67, 0x12, 0x07, 0x74, 0x65, 0x73, 0x74, 0x69, 0x6e, 0x67 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .Contain(t => t.Index == 1 && t.WireType == WireFormat.WireType.LengthDelimited)
                .And
                .Contain(t => t.Index == 2 && t.WireType == WireFormat.WireType.LengthDelimited);
        }

        [Fact]
        public void TwoVarintTags_TagTwoIsVarintWithValue1()
        {
            var input = new byte[] { 0x0a, 0x07, 0x74, 0x65, 0x73, 0x74, 0x69, 0x6e, 0x67, 0x12, 0x07, 0x74, 0x65, 0x73, 0x74, 0x69, 0x6e, 0x67 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .OfType<ProtobufTagSingle>()
                .Should()
                .Contain(t => t.Index == 2 && t.WireType == WireFormat.WireType.LengthDelimited && ((LengthDelimitedValue)t.Value).StringRepresentation == "testing");
        }
    }
}