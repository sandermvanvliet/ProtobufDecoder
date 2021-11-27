using System.Collections.Generic;

namespace ProtobufDecoder.Application.WinForms
{
    public class DataGridViewRowBuilder
    {
        private const int BytesPerLine = 16;

        public static List<ByteViewerRow> Build(byte[] input)
        {
            var offset = 0;
            var rows = new List<ByteViewerRow>();
            
            var remaining = input.Length;

            while (offset < input.Length)
            {
                var row = new ByteViewerRow
                {
                    Line = offset.ToString("X6")
                };

                if (remaining >= 1) { row.Byte1 = input[offset + 0].ToString("X2"); }
                if (remaining >= 2) { row.Byte2 = input[offset + 1].ToString("X2"); }
                if (remaining >= 3) { row.Byte3 = input[offset + 2].ToString("X2"); }
                if (remaining >= 4) { row.Byte4 = input[offset + 3].ToString("X2"); }
                if (remaining >= 5) { row.Byte5 = input[offset + 4].ToString("X2"); }
                if (remaining >= 6) { row.Byte6 = input[offset + 5].ToString("X2"); }
                if (remaining >= 7) { row.Byte7 = input[offset + 6].ToString("X2"); }
                if (remaining >= 8) { row.Byte8 = input[offset + 7].ToString("X2"); }
                if (remaining >= 9) { row.Byte9 = input[offset + 8].ToString("X2"); }
                if (remaining >= 10) { row.Byte10 = input[offset + 9].ToString("X2"); }
                if (remaining >= 11) { row.Byte11 = input[offset + 10].ToString("X2"); }
                if (remaining >= 12) { row.Byte12 = input[offset + 11].ToString("X2"); }
                if (remaining >= 13) { row.Byte13 = input[offset + 12].ToString("X2"); }
                if (remaining >= 14) { row.Byte14 = input[offset + 13].ToString("X2"); }
                if (remaining >= 15) { row.Byte15 = input[offset + 14].ToString("X2"); }
                if (remaining >= 16) { row.Byte16 = input[offset + 15].ToString("X2"); }

                rows.Add(row);
                
                offset += BytesPerLine;
                remaining -= BytesPerLine;
            }

            return rows;
        }
    }
}