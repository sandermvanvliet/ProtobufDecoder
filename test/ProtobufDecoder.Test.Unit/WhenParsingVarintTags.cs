using FluentAssertions;
using Google.Protobuf;
using Xunit;

namespace ProtobufDecoder.Test.Unit
{
    
    public class WhenParsingVarintTags
    {
        [Fact]
        public void SingleVarint_TagNumberIsOne()
        {
            var input = new byte[] { 0x08, 0x96, 0x01 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .OnlyContain(t => t.Index == 1);
        }

        [Fact]
        public void SingleVarint_WireTypeIsVarint()
        {
            var input = new byte[] { 0x08, 0x96, 0x01 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .OnlyContain(t => t.WireType == WireFormat.WireType.Varint);
        }

        [Fact]
        public void SingleVarint_ValueIsOfTypeVarint()
        {
            var input = new byte[] { 0x08, 0x96, 0x01 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .OnlyContain(t => t.Value is VarintValue);
        }

        [Fact]
        public void SingleVarint_ValueIsOfTypeVarintWithValue150()
        {
            var input = new byte[] { 0x08, 0x96, 0x01 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .OnlyContain(t => ((VarintValue)t.Value).Value == 150);
        }

        [Fact]
        public void TwoVarintTags_TwoTagsAreFound()
        {
            var input = new byte[] { 0x08, 0x96, 0x01, 0x10, 0x01 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .Contain(t => t.Index == 1 && t.WireType == WireFormat.WireType.Varint)
                .And
                .Contain(t => t.Index == 2 && t.WireType == WireFormat.WireType.Varint);
        }

        [Fact]
        public void TwoVarintTags_TagTwoIsVarintWithValue1()
        {
            var input = new byte[] { 0x08, 0x96, 0x01, 0x10, 0x01 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .Contain(t => t.Index == 2 && t.WireType == WireFormat.WireType.Varint && ((VarintValue)t.Value).Value == 1);
        }

        [Fact]
        public void TwoVarintTagsInReverseOrder_TwoTagsAreFound()
        {
            // Protobuf doesn't guarantee field order when serializing
            // so we need to be able to handle that.
            var input = new byte[] { 0x10, 0x01 , 0x08, 0x96, 0x01 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .Contain(t => t.Index == 1 && t.WireType == WireFormat.WireType.Varint)
                .And
                .Contain(t => t.Index == 2 && t.WireType == WireFormat.WireType.Varint);
        }
    }
}