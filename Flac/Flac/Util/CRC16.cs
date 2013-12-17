using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flac.Util
{
    /// <summary>
    /// Utility class to calculate 16-bit CRC.
    /// </summary>
    public static class CRC16
    {
        #region private static UInt16[] Crc16Table

        // CRC-16, poly = x^16 + x^15 + x^2 + x^0, init = 0
        private static UInt16[] Crc16Table =
            new UInt16[] {
                (UInt16) 0x0000,
                (UInt16) 0x8005,
                (UInt16) 0x800f,
                (UInt16) 0x000a,
                (UInt16) 0x801b,
                (UInt16) 0x001e,
                (UInt16) 0x0014,
                (UInt16) 0x8011,
                (UInt16) 0x8033,
                (UInt16) 0x0036,
                (UInt16) 0x003c,
                (UInt16) 0x8039,
                (UInt16) 0x0028,
                (UInt16) 0x802d,
                (UInt16) 0x8027,
                (UInt16) 0x0022,
                (UInt16) 0x8063,
                (UInt16) 0x0066,
                (UInt16) 0x006c,
                (UInt16) 0x8069,
                (UInt16) 0x0078,
                (UInt16) 0x807d,
                (UInt16) 0x8077,
                (UInt16) 0x0072,
                (UInt16) 0x0050,
                (UInt16) 0x8055,
                (UInt16) 0x805f,
                (UInt16) 0x005a,
                (UInt16) 0x804b,
                (UInt16) 0x004e,
                (UInt16) 0x0044,
                (UInt16) 0x8041,
                (UInt16) 0x80c3,
                (UInt16) 0x00c6,
                (UInt16) 0x00cc,
                (UInt16) 0x80c9,
                (UInt16) 0x00d8,
                (UInt16) 0x80dd,
                (UInt16) 0x80d7,
                (UInt16) 0x00d2,
                (UInt16) 0x00f0,
                (UInt16) 0x80f5,
                (UInt16) 0x80ff,
                (UInt16) 0x00fa,
                (UInt16) 0x80eb,
                (UInt16) 0x00ee,
                (UInt16) 0x00e4,
                (UInt16) 0x80e1,
                (UInt16) 0x00a0,
                (UInt16) 0x80a5,
                (UInt16) 0x80af,
                (UInt16) 0x00aa,
                (UInt16) 0x80bb,
                (UInt16) 0x00be,
                (UInt16) 0x00b4,
                (UInt16) 0x80b1,
                (UInt16) 0x8093,
                (UInt16) 0x0096,
                (UInt16) 0x009c,
                (UInt16) 0x8099,
                (UInt16) 0x0088,
                (UInt16) 0x808d,
                (UInt16) 0x8087,
                (UInt16) 0x0082,
                (UInt16) 0x8183,
                (UInt16) 0x0186,
                (UInt16) 0x018c,
                (UInt16) 0x8189,
                (UInt16) 0x0198,
                (UInt16) 0x819d,
                (UInt16) 0x8197,
                (UInt16) 0x0192,
                (UInt16) 0x01b0,
                (UInt16) 0x81b5,
                (UInt16) 0x81bf,
                (UInt16) 0x01ba,
                (UInt16) 0x81ab,
                (UInt16) 0x01ae,
                (UInt16) 0x01a4,
                (UInt16) 0x81a1,
                (UInt16) 0x01e0,
                (UInt16) 0x81e5,
                (UInt16) 0x81ef,
                (UInt16) 0x01ea,
                (UInt16) 0x81fb,
                (UInt16) 0x01fe,
                (UInt16) 0x01f4,
                (UInt16) 0x81f1,
                (UInt16) 0x81d3,
                (UInt16) 0x01d6,
                (UInt16) 0x01dc,
                (UInt16) 0x81d9,
                (UInt16) 0x01c8,
                (UInt16) 0x81cd,
                (UInt16) 0x81c7,
                (UInt16) 0x01c2,
                (UInt16) 0x0140,
                (UInt16) 0x8145,
                (UInt16) 0x814f,
                (UInt16) 0x014a,
                (UInt16) 0x815b,
                (UInt16) 0x015e,
                (UInt16) 0x0154,
                (UInt16) 0x8151,
                (UInt16) 0x8173,
                (UInt16) 0x0176,
                (UInt16) 0x017c,
                (UInt16) 0x8179,
                (UInt16) 0x0168,
                (UInt16) 0x816d,
                (UInt16) 0x8167,
                (UInt16) 0x0162,
                (UInt16) 0x8123,
                (UInt16) 0x0126,
                (UInt16) 0x012c,
                (UInt16) 0x8129,
                (UInt16) 0x0138,
                (UInt16) 0x813d,
                (UInt16) 0x8137,
                (UInt16) 0x0132,
                (UInt16) 0x0110,
                (UInt16) 0x8115,
                (UInt16) 0x811f,
                (UInt16) 0x011a,
                (UInt16) 0x810b,
                (UInt16) 0x010e,
                (UInt16) 0x0104,
                (UInt16) 0x8101,
                (UInt16) 0x8303,
                (UInt16) 0x0306,
                (UInt16) 0x030c,
                (UInt16) 0x8309,
                (UInt16) 0x0318,
                (UInt16) 0x831d,
                (UInt16) 0x8317,
                (UInt16) 0x0312,
                (UInt16) 0x0330,
                (UInt16) 0x8335,
                (UInt16) 0x833f,
                (UInt16) 0x033a,
                (UInt16) 0x832b,
                (UInt16) 0x032e,
                (UInt16) 0x0324,
                (UInt16) 0x8321,
                (UInt16) 0x0360,
                (UInt16) 0x8365,
                (UInt16) 0x836f,
                (UInt16) 0x036a,
                (UInt16) 0x837b,
                (UInt16) 0x037e,
                (UInt16) 0x0374,
                (UInt16) 0x8371,
                (UInt16) 0x8353,
                (UInt16) 0x0356,
                (UInt16) 0x035c,
                (UInt16) 0x8359,
                (UInt16) 0x0348,
                (UInt16) 0x834d,
                (UInt16) 0x8347,
                (UInt16) 0x0342,
                (UInt16) 0x03c0,
                (UInt16) 0x83c5,
                (UInt16) 0x83cf,
                (UInt16) 0x03ca,
                (UInt16) 0x83db,
                (UInt16) 0x03de,
                (UInt16) 0x03d4,
                (UInt16) 0x83d1,
                (UInt16) 0x83f3,
                (UInt16) 0x03f6,
                (UInt16) 0x03fc,
                (UInt16) 0x83f9,
                (UInt16) 0x03e8,
                (UInt16) 0x83ed,
                (UInt16) 0x83e7,
                (UInt16) 0x03e2,
                (UInt16) 0x83a3,
                (UInt16) 0x03a6,
                (UInt16) 0x03ac,
                (UInt16) 0x83a9,
                (UInt16) 0x03b8,
                (UInt16) 0x83bd,
                (UInt16) 0x83b7,
                (UInt16) 0x03b2,
                (UInt16) 0x0390,
                (UInt16) 0x8395,
                (UInt16) 0x839f,
                (UInt16) 0x039a,
                (UInt16) 0x838b,
                (UInt16) 0x038e,
                (UInt16) 0x0384,
                (UInt16) 0x8381,
                (UInt16) 0x0280,
                (UInt16) 0x8285,
                (UInt16) 0x828f,
                (UInt16) 0x028a,
                (UInt16) 0x829b,
                (UInt16) 0x029e,
                (UInt16) 0x0294,
                (UInt16) 0x8291,
                (UInt16) 0x82b3,
                (UInt16) 0x02b6,
                (UInt16) 0x02bc,
                (UInt16) 0x82b9,
                (UInt16) 0x02a8,
                (UInt16) 0x82ad,
                (UInt16) 0x82a7,
                (UInt16) 0x02a2,
                (UInt16) 0x82e3,
                (UInt16) 0x02e6,
                (UInt16) 0x02ec,
                (UInt16) 0x82e9,
                (UInt16) 0x02f8,
                (UInt16) 0x82fd,
                (UInt16) 0x82f7,
                (UInt16) 0x02f2,
                (UInt16) 0x02d0,
                (UInt16) 0x82d5,
                (UInt16) 0x82df,
                (UInt16) 0x02da,
                (UInt16) 0x82cb,
                (UInt16) 0x02ce,
                (UInt16) 0x02c4,
                (UInt16) 0x82c1,
                (UInt16) 0x8243,
                (UInt16) 0x0246,
                (UInt16) 0x024c,
                (UInt16) 0x8249,
                (UInt16) 0x0258,
                (UInt16) 0x825d,
                (UInt16) 0x8257,
                (UInt16) 0x0252,
                (UInt16) 0x0270,
                (UInt16) 0x8275,
                (UInt16) 0x827f,
                (UInt16) 0x027a,
                (UInt16) 0x826b,
                (UInt16) 0x026e,
                (UInt16) 0x0264,
                (UInt16) 0x8261,
                (UInt16) 0x0220,
                (UInt16) 0x8225,
                (UInt16) 0x822f,
                (UInt16) 0x022a,
                (UInt16) 0x823b,
                (UInt16) 0x023e,
                (UInt16) 0x0234,
                (UInt16) 0x8231,
                (UInt16) 0x8213,
                (UInt16) 0x0216,
                (UInt16) 0x021c,
                (UInt16) 0x8219,
                (UInt16) 0x0208,
                (UInt16) 0x820d,
                (UInt16) 0x8207,
                (UInt16) 0x0202 
            };
        #endregion

        /// <summary>
        /// Update the CRC with the byte data.
        /// </summary>
        /// <param name="data">The byte data</param>
        /// <param name="crc">The starting CRC value</param>
        /// <returns>The updated CRC value</returns>
        public static UInt16 Update(byte data, UInt16 crc)
        {
            crc = (UInt16)((crc << 8) ^ Crc16Table[((crc >> 8) ^ data) & 0xff]);
            return crc;
        }

        /// <summary>
        /// Update the CRC with the byte array data.
        /// </summary>
        /// <param name="data">The byte array data</param>
        /// <param name="len">The byte array length</param>
        /// <param name="crc">The starting CRC value</param>
        /// <returns>The updated CRC value</returns>
        public static UInt16 UpdateBlock(byte[] data, int len, UInt16 crc)
        {
            for (int i = 0; i < len; i++)
                crc = (UInt16)((crc << 8) ^ Crc16Table[(crc >> 8) ^ data[i]]);
            return crc;
        }

        /// <summary>
        /// Calculate the CRC over a byte array.
        /// </summary>
        /// <param name="data">The byte data</param>
        /// <param name="len">The byte array length</param>
        /// <returns>The calculated CRC value</returns>
        public static UInt16 Calculate(byte[] data, int len)
        {
            UInt16 crc = 0;

            for (int i = 0; i < len; i++)
                crc = (UInt16)((crc << 8) ^ Crc16Table[(crc >> 8) ^ data[i]]);

            return crc;
        }
    }
}
