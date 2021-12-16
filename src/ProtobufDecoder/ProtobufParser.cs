using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Google.Protobuf;
using ProtobufDecoder.Tags;
using ProtobufDecoder.Values;

namespace ProtobufDecoder
{
    public class ProtobufParser
    {
        public static MessageParseResult Parse(ReadOnlySpan<byte> input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input), "Input cannot be null");
            }

            var protobufMessage = new ProtobufMessage();

            if (input.Length == 0)
            {
                return MessageParseResult.Failed("Input was empty");
            }

            var protobufTags = new List<ProtobufTagSingle>();

            try
            {
                var index = 0;

                while (index < input.Length)
                {
                    ReadOnlySpan<byte> tagBytes;

                    // Because a tag field number can potentially be 29 bits
                    // and we know that it's encoded as a Varint it may
                    // happen that the field number needs 2 bytes to be 
                    // stored. 
                    // So we need to treat the tag as a proper varint (which
                    // it is) and then grab the field number and wire type
                    // from the resulting decoded Varint.
                    // See: https://stackoverflow.com/questions/57520857/maximum-field-number-in-protobuf-message
                    if ((input[index] & 0x80) != 0)
                    {
                        tagBytes = input.Slice(index, 2);
                        index++;
                    }
                    else
                    {
                        tagBytes = input.Slice(index, 1);
                    }

                    var tagAndWireType = (uint)VarintValue.ToTarget(tagBytes, 32).Item1.Value;

                    var tagFieldNumber = WireFormat.GetTagFieldNumber(tagAndWireType);

                    if (tagFieldNumber == 0)
                    {
                        return MessageParseResult.Failed("Tag with index 0 found which is not allowed");
                    }

                    var tag = new ProtobufTagSingle
                    {
                        Index = tagFieldNumber,
                        WireType = WireFormat.GetTagWireType(tagAndWireType),
                        StartOffset = index
                    };

                    protobufTags.Add(tag);

                    index += 1; // Advance to the content of the tag

                    switch (tag.WireType)
                    {
                        case WireFormat.WireType.Varint:
                            {
                                var parseResult = ParseVarint(input, index);
                                index += parseResult.Length;
                                tag.Value = parseResult.Value;
                                tag.DataOffset = parseResult.DataOffset;
                                tag.DataLength = parseResult.DataLength;
                                break;
                            }
                        case WireFormat.WireType.Fixed64:
                            var parseResultF = ParseFixed64(input, index);
                            index += parseResultF.Length;
                            tag.Value = parseResultF.Value;
                            tag.DataOffset = parseResultF.DataOffset;
                            tag.DataLength = parseResultF.DataLength;
                            break;
                        case WireFormat.WireType.LengthDelimited:
                            var parseResultL = ParseLengthDelimited(input, index);
                            index += parseResultL.Length;
                            tag.Value = parseResultL.Value;
                            tag.DataOffset = parseResultL.DataOffset;
                            tag.DataLength = parseResultL.DataLength;
                            break;
                        case WireFormat.WireType.StartGroup:
                            break;
                        case WireFormat.WireType.EndGroup:
                            break;
                        case WireFormat.WireType.Fixed32:
                            var parseResultF32 = ParseFixed32(input, index);
                            index += parseResultF32.Length;
                            tag.Value = parseResultF32.Value;
                            tag.DataOffset = parseResultF32.DataOffset;
                            tag.DataLength = parseResultF32.DataLength;
                            break;
                        default:
                            return MessageParseResult.Failed($"Invalid wire type {tag.WireType}");
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

                        var protobufTagRepeated = new ProtobufTagRepeated
                        {
                            Index = key,
                            WireType = firstTag.WireType
                        };

                        protobufTagRepeated.Items = new ObservableCollection<ProtobufTagSingle>(values
                            .Select(t =>
                            {
                                if (t is { WireType: WireFormat.WireType.LengthDelimited } singleTag)
                                {
                                    if (ProtobufTagLengthDelimited.IsProbableString(t.Value.RawValue))
                                    {
                                        t = ProtobufTagString.From(singleTag);
                                    }
                                    else
                                    {
                                        t = ProtobufTagLengthDelimited.From(singleTag);
                                    }
                                }

                                t.Parent = protobufTagRepeated;
                                return t;
                            })
                            .ToList());

                        return protobufTagRepeated;
                    })
                .ToList();

            // Change ProtobufTagSingle to ProtobufTagLengthDelimited for length-delimited tags
            groupedTags = groupedTags
                .Select(t =>
                {
                    if (t is ProtobufTagSingle singleTag && singleTag.WireType == WireFormat.WireType.LengthDelimited)
                    {
                        var isProbableString = ProtobufTagLengthDelimited.IsProbableString(singleTag.Value.RawValue);

                        if (!isProbableString)
                        {
                            var isProbablePackedFloat = ProtobufTagPacked.IsProbablePackedFloat(singleTag.Value.RawValue);
                            var isProbablePackedVarint = ProtobufTagPacked.IsProbablePackedVarint(singleTag.Value.RawValue);
                            
                            if (isProbablePackedFloat)
                            {
                                return ProtobufTagPackedFloat.From(singleTag);
                            }

                            if (isProbablePackedVarint)
                            {
                                return ProtobufTagPackedVarint.From(singleTag);
                            }
                        }

                        return ProtobufTagString.From(singleTag);
                    }

                    return t;
                })
                .ToList();

            protobufMessage.Tags.Clear();
            protobufMessage.Tags.AddRange(groupedTags);

            return MessageParseResult.Succeeded(protobufMessage);
        }

        private static ValueParseResult<LengthDelimitedValue> ParseLengthDelimited(ReadOnlySpan<byte> input, int index)
        {
            // Length-delimited tags are <tag number>|<varint length>|<data>
            var parsedLength = ParseVarint(input, index);

            index += parsedLength.Length;

            var valueLength = parsedLength.Value.AsUInt32();

            var fixedBytes = input.Slice(index, valueLength);

            return new ValueParseResult<LengthDelimitedValue>
            {
                Length = parsedLength.Length + valueLength, // The number of bytes for the length value + the length of the value itself
                DataLength = valueLength,
                Value = new LengthDelimitedValue(fixedBytes.ToArray()),
                DataOffset = index
            };
        }

        private static ValueParseResult<Fixed32Value> ParseFixed32(ReadOnlySpan<byte> input, int index)
        {
            if (index + 4 > input.Length)
            {
                throw new InvalidOperationException($"Expected 4 bytes but got {input.Length - (index + 4)}");
            }

            var fixedBytes = input.Slice(index, 4).ToArray();

            return new ValueParseResult<Fixed32Value>
            {
                Length = 4,
                Value = new Fixed32Value(fixedBytes),
                DataOffset = index,
                DataLength = 4
            };
        }

        private static ValueParseResult<Fixed64Value> ParseFixed64(ReadOnlySpan<byte> input, int index)
        {
            var fixedBytes = input.Slice(index, 8).ToArray();

            if (fixedBytes.Length != 8)
            {
                throw new InvalidOperationException($"Expected 8 bytes but got {fixedBytes.Length}");
            }

            return new ValueParseResult<Fixed64Value>
            {
                Length = 8,
                Value = new Fixed64Value(fixedBytes),
                DataOffset = index,
                DataLength = 8
            };
        }

        public static ValueParseResult<VarintValue> ParseVarint(ReadOnlySpan<byte> input, int index)
        {
            var length = 0;

            while (true)
            {
                var b = input[index + length];

                // Check if MSB is set, according to https://developers.google.com/protocol-buffers/docs/encoding#varints
                // the byte without MSB set indicates the last byte of a Base 128 Varint.
                if ((b & 0x80) == 0)
                {
                    break;
                }

                if (length > 16)
                {
                    throw new InvalidOperationException($"{length} is too many bytes to be a Varint");
                }

                length++;

                if (length >= input.Length)
                {
                    throw new InvalidOperationException($"Did not find enough bytes to parse Varint");
                }
            }

            length += 1;

            var varintBytes = input.Slice(index, length).ToArray();

            return new ValueParseResult<VarintValue>
            {
                Length = varintBytes.Length,
                Value = new VarintValue(varintBytes),
                DataOffset = index,
                DataLength = varintBytes.Length
            };
        }
    }
}