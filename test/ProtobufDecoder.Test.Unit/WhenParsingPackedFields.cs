using System.Linq;
using FluentAssertions;
using Google.Protobuf;
using Xunit;

namespace ProtobufDecoder.Test.Unit
{
    public class WhenParsingPackedFields
    {
        // Packed fields are hard to recognize because the
        // binary data tells us it's a length-delimited type
        // without telling us if it's a string, a nested message
        // or a packed array of the above.
        // Therefore parsing is the same as  parsing any other
        // length-delimited field and we need to do some smart
        // guesstimating on the LengthDelimitedValue type instead.

        [Fact]
        public void SinglePackedVarint_()
        {
            var input = new byte[] { 0x0a, 0x06, 0x03, 0x8e, 0x02, 0x9e, 0xa7, 0x05 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .OnlyContain(t => t.Index == 1);
        }

        [Fact]
        public void SinglePackedVarint_WireTypeIsLengthDelimited()
        {
            var input = new byte[] { 0x0a, 0x06, 0x03, 0x8e, 0x02, 0x9e, 0xa7, 0x05 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .OnlyContain(t => t.WireType == WireFormat.WireType.LengthDelimited);
        }

        [Fact]
        public void SinglePackedVarint_ValueIsOfTypeLengthDelimited()
        {
            var input = new byte[] { 0x0a, 0x06, 0x03, 0x8e, 0x02, 0x9e, 0xa7, 0x05 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .OfType<ProtobufTagSingle>()
                .Should()
                .OnlyContain(t => t.Value is LengthDelimitedValue);
        }

        [Fact]
        public void SinglePackedVarint_ValueIsOfTypeLengthDelimitedWithValue()
        {
            var input = new byte[] { 0x0a, 0x06, 0x03, 0x8e, 0x02, 0x9e, 0xa7, 0x05 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .OfType<ProtobufTagSingle>()
                .Single()
                .Value
                .As<LengthDelimitedValue>()
                .Value
                .Should()
                .BeEquivalentTo(new[] { 0x03, 0x8e, 0x02, 0x9e, 0xa7, 0x05 });
        }

        [Fact]
        public void UsingProtobufSerializedMessage_ParsesTwoTags()
        {
            var testMessage = new TestMessage
            {
                RepeatedInt32 = { 1, 2, 3, 4 },
                RepeatedString = { "a", "b", "cd", "efg" }
            };

            var bytes = testMessage.ToByteArray();

            var message = ProtobufParser.Parse(bytes);

            message
                .Tags
                .Should()
                .HaveCount(2);
        }

        [Fact]
        public void UsingProtobufSerializedMessage_SecondTagIsRepeatedString()
        {
            var testMessage = new TestMessage
            {
                RepeatedInt32 = { 1, 2, 3, 4 },
                RepeatedString = { "a", "b", "cd", "efg" }
            };

            var bytes = testMessage.ToByteArray();
            var message = ProtobufParser.Parse(bytes);

            message
                .Tags
                .Single(t => t.Index == 2)
                .As<ProtobufTagRepeated>()
                .Items
                .Should()
                .AllBeOfType<ProtobufTagString>();
        }
    }
}
