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

            var parseResult = ProtobufParser.Parse(input);

            parseResult
				.Message
                .Tags
                .Should()
                .OnlyContain(t => t.Index == 1);
        }

        [Fact]
        public void SingleString_WireTypeIsLengthDelimited()
        {
            var input = new byte[] { 0x0a, 0x07, 0x74, 0x65, 0x73, 0x74, 0x69, 0x6e, 0x67 };

            var parseResult = ProtobufParser.Parse(input);

            parseResult
				.Message
                .Tags
                .Should()
                .OnlyContain(t => t.WireType == WireFormat.WireType.LengthDelimited);
        }

        [Fact]
        public void SingleString_TagClassTypeIsProtobufTagString()
        {
            var input = new byte[] { 0x0a, 0x07, 0x74, 0x65, 0x73, 0x74, 0x69, 0x6e, 0x67 };

            var parseResult = ProtobufParser.Parse(input);

            parseResult
				.Message
                .Tags
                .Should()
                .OnlyContain(t => t is ProtobufTagString);
        }

        [Fact]
        public void SingleString_ValueIsOfTypeStringValue()
        {
            var input = new byte[] { 0x0a, 0x07, 0x74, 0x65, 0x73, 0x74, 0x69, 0x6e, 0x67 };

            var parseResult = ProtobufParser.Parse(input);

            parseResult
				.Message
                .Tags
                .OfType<ProtobufTagString>()
                .Should()
                .OnlyContain(t => t.Value is StringValue);
        }

        [Fact]
        public void SingleString_ValueIsTesting()
        {
            var input = new byte[] { 0x0a, 0x07, 0x74, 0x65, 0x73, 0x74, 0x69, 0x6e, 0x67 };

            var parseResult = ProtobufParser.Parse(input);

            parseResult
				.Message
                .Tags
                .OfType<ProtobufTagString>()
                .Single()
                .Value
                .As<StringValue>()
                .Value
                .Should()
                .Be("testing");
        }

        [Fact]
        public void TwoVarintTags_TwoTagsAreFound()
        {
            var input = new byte[] { 0x0a, 0x07, 0x74, 0x65, 0x73, 0x74, 0x69, 0x6e, 0x67, 0x12, 0x07, 0x74, 0x65, 0x73, 0x74, 0x69, 0x6e, 0x67 };

            var parseResult = ProtobufParser.Parse(input);

            parseResult
				.Message
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

            var parseResult = ProtobufParser.Parse(input);

            parseResult
				.Message
                .Tags
                .OfType<ProtobufTagSingle>()
                .Should()
                .Contain(t => t.Index == 2 && t.WireType == WireFormat.WireType.LengthDelimited && ((StringValue)t.Value).Value == "testing");
        }

        [Fact]
        public void GivenSingleTagWithTypeLengthDelimited_AllPropertiesAreSetOnLengthDelimitedTag()
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

        [Fact]
        public void GivenSingleTagWithStringConten_AllPropertiesAreSetOnStringTag()
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

            var lengthDelimitedTag = ProtobufTagString.From(singleTag);

            lengthDelimitedTag
                .Should()
                .BeEquivalentTo(
                    singleTag,
                    options => options.Excluding(_ => _.Value.CanDecode)); // Exclude CanDecode because we're changing to a StringValue which can't be further decoded
        }
    }
}