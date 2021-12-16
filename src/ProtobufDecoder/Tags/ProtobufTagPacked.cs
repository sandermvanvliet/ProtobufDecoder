using Google.Protobuf;

namespace ProtobufDecoder.Tags
{
    public abstract class ProtobufTagPacked : ProtobufTagSingle
    {
        public static bool IsProbablePackedVarint(byte[] input)
        {
            var index = 0;

            try
            {
                while (index < input.Length)
                {
                    var parseResult = ProtobufParser.ParseVarint(input, index);
                
                    index += parseResult.Length;
                }

                if (index == input.Length)
                {
                    return !StartsWithATag(input);
                }
            }
            catch
            {
                // ignored
            }

            return false;
        }

        public static bool IsProbablePackedFloat(byte[] input)
        {
            if (input.Length < 4)
            {
                return false;
            }

            // Multiples of 4 bytes
            return input.Length > 4 && input.Length % 4 == 0 && !StartsWithATag(input);
        }

        public static bool IsProbablePackedDouble(byte[] input)
        {
            // Multiples of 8 bytes
            return input.Length > 8 && input.Length % 8 == 0 && !StartsWithATag(input);
        }

        private static bool StartsWithATag(byte[] input)
        {
            return WireFormat.GetTagFieldNumber(input[0]) > 0 &&
                   (int)WireFormat.GetTagWireType(input[0]) is >= 0 and <= 5;
        }
    }
}