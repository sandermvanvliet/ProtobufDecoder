using System.Linq;
using FluentAssertions;
using Google.Protobuf;
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
        public void RepeatedLengthDelimited_ValueIsOfTypeRepeated()
        {
            var input = new byte[] { 0x0a, 0x01, 0x03, 0x0a, 0x01, 0x03, 0x0a, 0x01, 0x03 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .OnlyContain(t => t.Value is RepeatedValue);
        }

        [Fact]
        public void RepeatedLengthDelimited_ValueIsOfTypeRepeatedWithThreeValues()
        {
            var input = new byte[] { 0x0a, 0x01, 0x03, 0x0a, 0x01, 0x03, 0x0a, 0x01, 0x03 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Single()
                .Value
                .As<RepeatedValue>()
                .Value
                .Should()
                .HaveCount(3);
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
        public void RepeatedVarints_ValueIsOfTypeRepeated()
        {
            var input = new byte[] { 0x08, 0x03, 0x08, 0x03, 0x08, 0x03 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .OnlyContain(t => t.Value is RepeatedValue);
        }

        [Fact]
        public void RepeatedVarints_ValueIsOfTypeRepeatedWithThreeValues()
        {
            var input = new byte[] { 0x08, 0x03, 0x08, 0x03, 0x08, 0x03 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Single()
                .Value
                .As<RepeatedValue>()
                .Value
                .Should()
                .OnlyContain(v => v is VarintValue)
                .And
                .OnlyContain(v => ((VarintValue)v).Value == 3);
        }
    }
}
