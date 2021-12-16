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
                    return true;
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
            return input.Length > 4 && input.Length % 4 == 0;
        }

        public static bool IsProbablePackedDouble(byte[] input)
        {
            // Multiples of 8 bytes
            return input.Length > 8 && input.Length % 8 == 0;
        }
    }
}