using System;
using System.Linq;
using FluentAssertions;
using Google.Protobuf;
using ProtobufDecoder.Values;
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
                .OfType<ProtobufTagSingle>()
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
                .OfType<ProtobufTagSingle>()
                .Should()
                .OnlyContain(t => ((VarintValue)t.Value).UInt16 == "150");
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
                .OfType<ProtobufTagSingle>()
                .Should()
                .Contain(t => t.Index == 2 && t.WireType == WireFormat.WireType.Varint && ((VarintValue)t.Value).UInt16 == "1");
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

        [Fact]
        public void UnsignedShort()
        {
            var input = new byte[] { 0x10, 0x79 };

            var message = ProtobufParser.Parse(input);

            var varintValue = message.Tags.Single().As<ProtobufTagSingle>().Value.As<VarintValue>();

            varintValue.Bool.Should().Be("Not a boolean");
            varintValue.Int16.Should().Be("-61");
            varintValue.UInt16.Should().Be("121");
            varintValue.Int32.Should().Be("-61");
            varintValue.UInt32.Should().Be("121");
            varintValue.Int64.Should().Be("-61");
            varintValue.UInt64.Should().Be("121");
        }

        [Fact]
        public void UnsignedInteger()
        {
            var input = new byte[] { 0x10 }.Concat(VarintBitConverter.GetVarintBytes((uint)100000000)).ToArray();

            var message = ProtobufParser.Parse(input);

            var varintValue = message.Tags.Single().As<ProtobufTagSingle>().Value.As<VarintValue>();

            varintValue.Bool.Should().Be("Not a boolean");
            varintValue.Int16.Should().Be("Got too many bytes to represent this value");
            varintValue.UInt16.Should().Be("Got too many bytes to represent this value");
            varintValue.Int32.Should().Be("50000000"); // Because zig-zag encoding
            varintValue.UInt32.Should().Be("100000000");
            varintValue.Int64.Should().Be("50000000"); // Because zig-zag encoding
            varintValue.UInt64.Should().Be("100000000");
        }

        [Fact]
        public void UnsignedLong()
        {
            var input = new byte[] { 0x10 }.Concat(VarintBitConverter.GetVarintBytes((ulong)1000000000000)).ToArray();

            var message = ProtobufParser.Parse(input);

            var varintValue = message.Tags.Single().As<ProtobufTagSingle>().Value.As<VarintValue>();

            varintValue.Bool.Should().Be("Not a boolean");
            varintValue.Int16.Should().Be("Got too many bytes to represent this value");
            varintValue.UInt16.Should().Be("Got too many bytes to represent this value");
            varintValue.Int32.Should().Be("Got too many bytes to represent this value");
            varintValue.UInt32.Should().Be("Got too many bytes to represent this value");
            varintValue.Int64.Should().Be("500000000000"); // Because zig-zag encoding
            varintValue.UInt64.Should().Be("1000000000000");
        }
    }
}