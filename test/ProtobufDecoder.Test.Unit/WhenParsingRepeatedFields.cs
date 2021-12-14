using System.Linq;
using FluentAssertions;
using Google.Protobuf;
using ProtobufDecoder.Tags;
using ProtobufDecoder.Values;
using Xunit;

namespace ProtobufDecoder.Test.Unit
{
    
    public class WhenParsingRepeatedFields
    {
        // Packed fields are hard to recognize because the
        // binary data tells us it's a length-delimited type
        // without telling us if it's a string, a nested message
        // or a packed array of the above.
        // Therefore parsing is the same as  parsing any other
        // length-delimited field and we need to do some smart
        // guesstimating on the LengthDelimitedValue type instead.

        [Fact]
        public void RepeatedLengthDelimited_MessageContainsSingleTag()
        {
            var input = new byte[] { 0x0a, 0x01, 0x03, 0x0a, 0x01, 0x03, 0x0a, 0x01, 0x03 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .OnlyContain(t => t.Index == 1)
                .And
                .HaveCount(1);
        }

        [Fact]
        public void RepeatedLengthDelimited_WireTypeIsLengthDelimited()
        {
            var input = new byte[] { 0x0a, 0x01, 0x03, 0x0a, 0x01, 0x03, 0x0a, 0x01, 0x03 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .OnlyContain(t => t.WireType == WireFormat.WireType.LengthDelimited);
        }

        [Fact]
        public void RepeatedLengthDelimited_TagIsARepeatedTag()
        {
            var input = new byte[] { 0x0a, 0x01, 0x03, 0x0a, 0x01, 0x03, 0x0a, 0x01, 0x03 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .AllBeOfType<ProtobufTagRepeated>();
        }

        [Fact]
        public void RepeatedLengthDelimited_TagHasThreeItems()
        {
            var input = new byte[] { 0x0a, 0x01, 0x03, 0x0a, 0x01, 0x03, 0x0a, 0x01, 0x03 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Single()
                .As<ProtobufTagRepeated>()
                .Items
                .Should()
                .HaveCount(3);
        }

        [Fact]
        public void RepeatedLengthDelimited_ItemsAreLengthDelimitedTags()
        {
            var input = new byte[] { 0x0a, 0x01, 0x03, 0x0a, 0x01, 0x03, 0x0a, 0x01, 0x03 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Single()
                .As<ProtobufTagRepeated>()
                .Items
                .Should()
                .AllBeOfType<ProtobufTagLengthDelimited>();
        }

        [Fact]
        public void RepeatedLengthDelimited_ItemInstanceHasVarintValueOf3()
        {
            var input = new byte[] { 0x0a, 0x01, 0x03, 0x0a, 0x01, 0x03, 0x0a, 0x01, 0x03 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Single()
                .As<ProtobufTagRepeated>()
                .Items
                .Should()
                .AllBeOfType<ProtobufTagLengthDelimited>()
                .Which
                .First()
                .Value
                .As<LengthDelimitedValue>()
                .Value
                .Should()
                .BeEquivalentTo(new byte[] { 0x3 });
        }

        [Fact]
        public void RepeatedVarints_MessageContainsSingleTag()
        {
            var input = new byte[] { 0x08, 0x03, 0x08, 0x03, 0x08, 0x03 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .OnlyContain(t => t.Index == 1)
                .And
                .HaveCount(1);
        }

        [Fact]
        public void RepeatedVarints_WireTypeIsVarint()
        {
            var input = new byte[] { 0x08, 0x03, 0x08, 0x03, 0x08, 0x03 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .OnlyContain(t => t.WireType == WireFormat.WireType.Varint);
        }

        [Fact]
        public void RepeatedVarint_ItemInstanceHasVarintValueOf3()
        {
            var input = new byte[] { 0x08, 0x03, 0x08, 0x03, 0x08, 0x03 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Single()
                .As<ProtobufTagRepeated>()
                .Items
                .Should()
                .AllBeOfType<ProtobufTagSingle>()
                .And
                .OnlyContain(t => ((VarintValue)t.Value).AsUInt32() == 3);
        }
    }
}
