using System;

namespace ProtobufDecoder.Application.WinForms
{
    internal class ByteViewerRow
    {
        public ByteViewerRow(byte[] rawBytes, int line)
        {
            Byte1 = rawBytes[0].ToString("X2");
            Byte2 = rawBytes[1].ToString("X2");
            Byte3 = rawBytes[2].ToString("X2");
            Byte4 = rawBytes[3].ToString("X2");
            Byte5 = rawBytes[4].ToString("X2");
            Byte6 = rawBytes[5].ToString("X2");
            Byte7 = rawBytes[6].ToString("X2");
            Byte8 = rawBytes[7].ToString("X2");
            Byte9 = rawBytes[8].ToString("X2");
            Byte10 = rawBytes[9].ToString("X2");
            Byte11 = rawBytes[10].ToString("X2");
            Byte12 = rawBytes[11].ToString("X2");
            Byte13 = rawBytes[12].ToString("X2");
            Byte14 = rawBytes[13].ToString("X2");
            Byte15 = rawBytes[14].ToString("X2");
            Byte16 = rawBytes[15].ToString("X2");
            Line = line;
        }

        public int Line { get; set; }
        public string Byte1 { get; }
        public string Byte2 { get; }
        public string Byte3 { get; }
        public string Byte4 { get; }
        public string Byte5 { get; }
        public string Byte6 { get; }
        public string Byte7 { get; }
        public string Byte8 { get; }
        public string Byte9 { get; }
        public string Byte10 { get; }
        public string Byte11 { get; }
        public string Byte12 { get; }
        public string Byte13 { get; }
        public string Byte14 { get; }
        public string Byte15 { get; }
        public string Byte16 { get; }
    }
}