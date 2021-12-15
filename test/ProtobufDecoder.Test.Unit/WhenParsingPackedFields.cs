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
        public void SinglePackedVarint_WireTypeIsVarint()
        {
            var input = new byte[] { 0x0a, 0x06, 0x03, 0x8e, 0x02, 0x9e, 0xa7, 0x05 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .OnlyContain(t => t.WireType == WireFormat.WireType.Varint);
        }

        [Fact]
        public void SinglePackedVarint_TagIsOtTypePackedProtobufTag()
        {
            var input = new byte[] { 0x0a, 0x06, 0x03, 0x8e, 0x02, 0x9e, 0xa7, 0x05 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .Should()
                .OnlyContain(t => t is ProtobufTagPackedVarint);
        }

        [Fact]
        public void SinglePackedVarint_PackedTagHasThreeValues()
        {
            var input = new byte[] { 0x0a, 0x06, 0x03, 0x8e, 0x02, 0x9e, 0xa7, 0x05 };

            var message = ProtobufParser.Parse(input);

            message
                .Tags
                .OfType<ProtobufTagPackedVarint>()
                .Single()
                .Values
                .Should()
                .HaveCount(3);
        }

        [Fact]
        public void SinglePackedVarint_PackedTagLengthIsTotalLength()
        {
            var input = new byte[] { 0x0a, 0x06, 0x03, 0x8e, 0x02, 0x9e, 0xa7, 0x05 };

            var message = ProtobufParser.Parse(input);

            var tag = message.Tags.Single() as ProtobufTagPackedVarint;

            tag.Length.Should().Be(8);
        }

        [Fact]
        public void SinglePackedVarint_PackedTagDataLengthIsSix()
        {
            var input = new byte[] { 0x0a, 0x06, 0x03, 0x8e, 0x02, 0x9e, 0xa7, 0x05 };

            var message = ProtobufParser.Parse(input);

            var tag = message.Tags.Single() as ProtobufTagPackedVarint;

            tag.DataLength.Should().Be(6);
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
        public void UsingProtobufSerializedMessage_FirstTagIsRepeatedVarInt()
        {
            var testMessage = new TestMessage
            {
                RepeatedInt32 = { 1, 2, 3, 4 },
                //RepeatedString = { "a", "b", "cd", "efg" }
            };

            var bytes = testMessage.ToByteArray();

            var message = ProtobufParser.Parse(bytes);

            message
                .Tags
                .Single(t => t.Index == 1)
                .Should()
                .BeOfType<ProtobufTagPackedVarint>()
                .Which
                .Values
                .Should()
                .HaveCount(4);
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
                .AllBeOfType<ProtobufTagString>()
                .And
                .HaveCount(4);
        }

        [Fact]
        public void GivenSingleTagWithPackedVarints_AllPropertiesAreSetOnPackedTag()
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
                    p.SetValue(singleTag, new LengthDelimitedValue(new byte[] { 116 })); // This value needs to be fixed because it needs to be a valid Varint
                }
            }

            var packedTag = ProtobufTagPackedVarint.PackedVarIntFrom(singleTag);

            packedTag
                .Should()
                .BeEquivalentTo(
                    singleTag,
                    options => options.Excluding(_ => _.Value)); // Exclude CanDecode because we're changing to a StringValue which can't be further decoded
        }
    }
}
