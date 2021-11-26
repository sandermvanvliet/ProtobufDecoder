using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;

namespace ProtobufDecoder
{
    public class ProtobufParser
    {
        public static ProtobufMessage Parse(ReadOnlySpan<byte> input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input), "Input cannot be null");
            }

            var protobufMessage = new ProtobufMessage();

            if (input.Length == 0)
            {
                return protobufMessage;
            }

            var protobufTags = new List<ProtobufTagSingle>();

            try
            {
                var index = 0;

                while (index < input.Length)
                {
                    var tag = new ProtobufTagSingle
                    {
                        Index = WireFormat.GetTagFieldNumber(input[index]),
                        WireType = WireFormat.GetTagWireType(input[index]),
                        StartOffset = index
                    };

                    protobufTags.Add(tag);

                    index += 1; // This is the tag + wire type byte

                    switch (tag.WireType)
                    {
                        case WireFormat.WireType.Varint:
                        {
                            var parseResult = ParseVarint(input, index);
                            index += parseResult.Length;
                            tag.Value = parseResult.Value;
                            break;
                        }
                        case WireFormat.WireType.Fixed64:
                            var parseResultF = ParseFixed64(input, index);
                            index += parseResultF.Length;
                            tag.Value = parseResultF.Value;
                            break;
                        case WireFormat.WireType.LengthDelimited:
                            var parseResultL = ParseLengthDelimited(input, index);
                            index += parseResultL.Length;
                            tag.Value = parseResultL.Value;
                            break;
                        case WireFormat.WireType.StartGroup:
                            break;
                        case WireFormat.WireType.EndGroup:
                            break;
                        case WireFormat.WireType.Fixed32:
                            var parseResultF32 = ParseFixed32(input, index);
                            index += parseResultF32.Length;
                            tag.Value = parseResultF32.Value;
                            break;
                        default:
                            throw new InvalidOperationException($"Invalid wire type {input[index]}");
                    }

                    tag.EndOffset = index - 1; // Subtract 1 because index is pointing at the start byte of the tag after the current one
                }
            }
            catch (InvalidOperationException)
            {
                // We terminate parsing on an InvalidOperationException so
                // we don't know where the currently parsed tag ends.
                // Therefore mark it as the end of the payload because we
                // can't do better.
                protobufTags.Last().EndOffset = input.Length;
            }

            // Do some special magic to handle repeated fields.
            // These are length-delimited fields where we have more than
            // one occurrence for the same tag.
            // Note: Non-packed repeated tags can appear anywhere in the byte
            //       stream so this grouping destroys the offsets....
            var groupedTags = protobufTags
                .GroupBy<ProtobufTagSingle, int, ProtobufTagSingle, ProtobufTag>(
                    tag => tag.Index,
                    tag => tag,
                    (key, values) =>
                    {
                        if (values.Count() == 1)
                        {
                            return values.First();
                        }

                        var firstTag = values.First();

                        return new ProtobufTagRepeated
                        {
                            Index = key,
                            WireType = firstTag.WireType,
                            Items = values.ToArray()
                        };
                    })
                .ToList();

            protobufMessage.Tags.Clear();
            protobufMessage.Tags.AddRange(groupedTags);

            return protobufMessage;
        }

        private static ParseResult<LengthDelimitedValue> ParseLengthDelimited(ReadOnlySpan<byte> input, int index)
        {
            // Length-delimited tags are <tag number>|<varint length>|<data>
            var parsedLength = ParseVarint(input, index);

            index += parsedLength.Length;

            var valueLength = parsedLength.Value.AsUInt32();

            var fixedBytes = input.Slice(index, valueLength);

            return new ParseResult<LengthDelimitedValue>
            {
                Length = parsedLength.Length + valueLength, // The number of bytes for the length value + the length of the value itself
                Value = new LengthDelimitedValue(fixedBytes.ToArray())
            };
        }

        private static ParseResult<Fixed32Value> ParseFixed32(ReadOnlySpan<byte> input, int index)
        {
            if (index + 4 > input.Length)
            {
                throw new InvalidOperationException($"Expected 4 bytes but got {input.Length - (index + 4)}");
            }

            var fixedBytes = input.Slice(index, 4).ToArray();

            return new ParseResult<Fixed32Value>
            {
                Length = 4,
                Value = new Fixed32Value(fixedBytes)
            };
        }

        private static ParseResult<Fixed64Value> ParseFixed64(ReadOnlySpan<byte> input, int index)
        {
            var fixedBytes = input.Slice(index, 8).ToArray();

            if (fixedBytes.Length != 8)
            {
                throw new InvalidOperationException($"Expected 8 bytes but got {fixedBytes.Length}");
            }

            return new ParseResult<Fixed64Value>
            {
                Length = 8,
                Value = new Fixed64Value(fixedBytes)
            };
        }

        private static ParseResult<VarintValue> ParseVarint(ReadOnlySpan<byte> input, int index)
        {
            var length = 0;

            while (true)
            {
                var b = input[index + length];
                
                // Check if MSB is set, according to https://developers.google.com/protocol-buffers/docs/encoding#varints
                // the byte with MSB set indicates the last byte of a Base 128 Varint.
                if ((b & 0x80) == 0)
                {
                    break;
                }

                if (length > 16)
                {
                    throw new InvalidOperationException($"{length} is too many bytes to be a Varint");
                }

                length++;
            }

            length += 1;

            var varintBytes = input.Slice(index, length).ToArray();

            return new ParseResult<VarintValue>
            {
                Length = varintBytes.Length,
                Value = new VarintValue(varintBytes)
            };
        }
    }
}