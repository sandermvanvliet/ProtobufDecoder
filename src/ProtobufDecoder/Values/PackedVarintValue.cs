using System.Collections.Generic;
using System.Linq;

namespace ProtobufDecoder.Values
{
    public class PackedVarintValue : ProtobufValue
    {
        public PackedVarintValue(byte[] input)
        {
            Values = ExplodeVarInts(input);
        }

        public VarintValue[] Values { get; set; }
        public override bool CanDecode => false;

        private static VarintValue[] ExplodeVarInts(byte[] input)
        {
            var list = new List<VarintValue>();
            var index = 0;

            while (index < input.Length)
            {
                var parseResult = ProtobufParser.ParseVarint(input, index);

                list.Add(parseResult.Value);

                index += parseResult.Length;
            }

            return list.ToArray();
        }

        public override string ToString()
        {
            return string.Join(", ", Values.Select(v => v.UInt32));
        }
    }
}