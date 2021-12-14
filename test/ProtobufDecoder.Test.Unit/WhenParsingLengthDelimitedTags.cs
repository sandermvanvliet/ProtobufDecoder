using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Google.Protobuf;
using ProtobufDecoder.Tags;
using ProtobufDecoder.Values;
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
        public void SingleString_TagClassTypeIsProtobufTagLengthDelimited()
        {
            var input = new byte[] { 0x0a, 0x07, 0x74, 0x65, 0x73, 0x74, 0x69, 0x6e, 0x67 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .OnlyContain(t => t is ProtobufTagLengthDelimited);
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

        [Fact]
        public void CloneConstructorWorks()
        {
            var singleTag = new ProtobufTagSingle();
            var random = new Random();
            var properties = singleTag
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.SetMethod != null)
                .ToList();

            foreach (var p in properties)
            {
                if (p.PropertyType == typeof(string))
                {
                    p.SetValue(singleTag, "str" + random.Next(1, 1000));
                }
                else if (p.PropertyType == typeof(int) || p.PropertyType == typeof(short) ||
                         p.PropertyType == typeof(long))
                {
                    p.SetValue(singleTag, random.Next(-1000, -1));
                }
                else if (p.PropertyType == typeof(ProtobufValue))
                {
                    p.SetValue(singleTag, new LengthDelimitedValue(new byte[] { (byte)random.Next(1001, 2000) }));
                }
            }

            var lengthDelimitedTag = ProtobufTagLengthDelimited.From(singleTag);

            lengthDelimitedTag
                .Should()
                .BeEquivalentTo(singleTag);
        }
    }
}