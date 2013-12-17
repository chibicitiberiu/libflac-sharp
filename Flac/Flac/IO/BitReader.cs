using Flac.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Flac.IO
{
    public class BitReader
    {
        #region Constants
        private const int DefaultCapacity = 2048;
        private const int BitsPerByte = 8;
        private const int BitsPerByteLog2 = 3;
        private const byte ByteTopBitOne = (byte)0x80;
        #endregion

        #region Private members
        private byte[] buffer;
        private int putByte = 0;
        private int getByte = 0;
        private int getBit = 0;
        private int availBits = 0;
        private int totalBitsRead = 0;
        private UInt16 readCrc16 = 0;
        private Stream stream;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of BitReader
        /// </summary>
        /// <param name="stream">Input data stream</param>
        public BitReader(Stream stream)
        {
            buffer = new byte[DefaultCapacity];

            this.stream = stream;
        }
        #endregion

        /// <summary>
        /// Reads data from stream.
        /// </summary>
        /// <returns>Number of bytes read</returns>
        private int ReadFromStream()
        {
            // first shift the unconsumed buffer data toward the front as much as possible
            if (getByte > 0 && putByte > getByte)
            {
                Array.Copy(buffer, getByte, buffer, 0, putByte - getByte);
            }
            putByte -= getByte;
            getByte = 0;

            // Set the target for reading, taking into account byte alignment
            int bytes = buffer.Length - putByte;

            // Read some data
            bytes = stream.Read(buffer, putByte, bytes);
            if (bytes <= 0)
                throw new IOException("Failed to read from buffer.");

            // Now we have to handle partial byte cases
            putByte += bytes;
            availBits += bytes << 3;
            return bytes;
        }

        /// <summary>
        /// Reset the bit stream.
        /// </summary>
        public void Reset()
        {
            getByte = 0;
            getBit = 0;
            putByte = 0;
            availBits = 0;
        }

        /// <summary>
        /// Reset the read CRC-16 value.
        /// </summary>
        /// <param name="seed">The initial CRC-16 value</param>
        public void ResetReadCrc16(UInt16 seed)
        {
            readCrc16 = seed;
        }

        /// <summary>
        /// Gets the the read CRC-16 value.
        /// </summary>
        public UInt16 ReadCrc16
        {
            get
            {
                return readCrc16;
            }
        }

        /// <summary>
        /// Test if the Bit Stream consumed bits is byte aligned.
        /// </summary>
        public bool IsConsumedByteAligned
        {
            get
            {
                return ((getBit & 0x7) == 0);
            }
        }

        /// <summary>
        /// Gets the number of bits to read to align the byte.
        /// </summary>
        public int BitsLeftForByteAlignment
        {
            get
            {
                return 8 - (getBit & 7);
            }
        }

        /// <summary>
        /// Gets the number of bytes left to read.
        /// </summary>
        public int InputBitsUnconsumed
        {
            get
            {
                return availBits >> 3;
            }
        }

        /// <summary>
        /// Skips over bits in bit stream without updating CRC.
        /// </summary>
        /// <param name="bits">Number of bits to skip</param>
        /// <exception cref="IOException">Thrown if error reading from input stream</exception>
        public void SkipBitsNoCrc(int bits)
        {
            if (bits > 0)
            {
                int bitsToAlign = getBit & 7;
                if (bitsToAlign != 0)
                {
                    int bitsToTake = Math.Min(8 - bitsToAlign, bits);
                    ReadRawUInt32(bitsToTake);
                    bits -= bitsToTake;
                }

                int bytesNeeded = bits / 8;
                if (bytesNeeded > 0)
                {
                    ReadByteBlockAlignedNoCrc(null, bytesNeeded);
                    bits %= 8;
                }

                if (bits > 0)
                {
                    ReadRawUInt32(bits);
                }
            }
        }

        /// <summary>
        /// Reads a single bit.
        /// </summary>
        /// <returns>The bit</returns>
        /// <exception cref="IOException">Thrown if error reading input stream</exception>
        public int ReadBit()
        {
            while (availBits <= 0)
            {
                ReadFromStream();
            }

            int val = ((buffer[getByte] & (0x80 >> getBit)) != 0) ? 1 : 0;
            getBit++;
            if (getBit == BitsPerByte)
            {
                readCrc16 = CRC16.Update(buffer[getByte], readCrc16);
                getByte++;
                getBit = 0;
            }
            availBits--;
            totalBitsRead++;
            return val;
        }

        /// <summary>
        /// Read a bit into an integer value. 
        /// The bits of the input integer are shifted left and the 
        /// bit is placed into bit 0.
        /// </summary>
        /// <param name="val">The integer to shift and add read bit</param>
        /// <returns>The updated integer value</returns>
        /// <exception cref="IOException">Thrown if error reading input stream</exception>
        public UInt32 ReadBitToUInt32(UInt32 val)
        {
            while (availBits <= 0)
            {
                ReadFromStream();
            }

            val <<= 1;
            val |= ((buffer[getByte] & (0x80 >> getBit)) != 0) ? 1u : 0u;
            getBit++;
            if (getBit == BitsPerByte)
            {
                readCrc16 = CRC16.Update(buffer[getByte], readCrc16);
                getByte++;
                getBit = 0;
            }
            availBits--;
            totalBitsRead++;
            return val;
        }

        /// <summary>
        /// Peeks at the next bit and add it to the input integer.
        /// The bits of the input integer are shifted left and the 
        /// bit is placed into bit 0.
        /// </summary>
        /// <param name="val">The input integer</param>
        /// <param name="bit">The bit to peek at</param>
        /// <returns>The updated integer value</returns>
        /// <exception cref="IOException">Thrown if error reading input stream</exception>
        public UInt32 PeekBitToUInt32(UInt32 val, int bit)
        {
            while (availBits <= 0)
            {
                ReadFromStream();
            }

            val <<= 1;
            if ((getBit + bit) >= BitsPerByte)
            {
                bit = (getBit + bit) % BitsPerByte;
                val |= ((buffer[getByte + 1] & (0x80 >> bit)) != 0) ? 1u : 0u;
            }
            else
            {
                val |= ((buffer[getByte] & (0x80 >> (getBit + bit))) != 0) ? 1u : 0u;
            }
            return val;
        }

        /// <summary>
        /// Read a bit into a long value. 
        /// The bits of the input integer are shifted left and the 
        /// bit is placed into bit 0.
        /// </summary>
        /// <param name="val">The long to shift and add read bit</param>
        /// <returns>The updated long value</returns>
        /// <exception cref="IOException">Thrown if error reading input stream</exception>
        public UInt64 ReadBitToUInt64(UInt64 val)
        {
            while (availBits <= 0)
            {
                ReadFromStream();
            }

            val <<= 1;
            val |= ((buffer[getByte] & (0x80 >> getBit)) != 0) ? 1u : 0u;
            getBit++;
            if (getBit == BitsPerByte)
            {
                readCrc16 = CRC16.Update(buffer[getByte], readCrc16);
                getByte++;
                getBit = 0;
            }
            availBits--;
            totalBitsRead++;
            return val;
        }

        /// <summary>
        /// Read bits into an unsigned integer.
        /// </summary>
        /// <param name="bits">The number of bits to read</param>
        /// <returns>The bits as an unsigned integer</returns>
        /// <exception cref="IOException">Thrown if error reading input stream</exception>
        public UInt32 ReadRawUInt32(int bits)
        {
            UInt32 val = 0;
            for (int i = 0; i < bits; i++)
            {
                val = ReadBitToUInt32(val);
            }
            return val;
        }

        /// <summary>
        /// Peek at bits into an unsigned integer without advancing the input stream.
        /// </summary>
        /// <param name="bits">The number of bits to read</param>
        /// <returns>The bits as an unsigned integer</returns>
        /// <exception cref="IOException">Thrown if error reading input stream</exception>
        public UInt32 PeekRawUInt32(int bits)
        {
            UInt32 val = 0;
            for (int i = 0; i < bits; i++)
            {
                val = PeekBitToUInt32(val, i);
            }
            return val;
        }

        /// <summary>
        /// Read bits into an unsigned long.
        /// </summary>
        /// <param name="bits">The number of bits to read</param>
        /// <returns>The bits as an unsigned long</returns>
        /// <exception cref="IOException">Thrown if error reading input stream</exception>
        public UInt64 ReadRawUInt64(int bits)
        {
            UInt64 val = 0;
            for (int i = 0; i < bits; i++)
            {
                val = ReadBitToUInt64(val);
            }
            return val;
        }

        /// <summary>
        /// Read bits into an unsigned little endian integer.
        /// </summary>
        /// <returns>The bits as an unsigned integer</returns>
        /// <exception cref="IOException">Thrown if error reading input stream</exception>
        public UInt32 ReadRawUInt32LittleEndian()
        {
            UInt32 val = ReadRawUInt32(8);
            UInt32 a = ReadRawUInt32(8);
            UInt32 b = ReadRawUInt32(8);
            UInt32 c = ReadRawUInt32(8);

            return (val | (a << 8) | (b << 16) | (c << 24));
        }

        /// <summary>
        /// Read a block of bytes (aligned) without updating the CRC value.
        /// </summary>
        /// <param name="val">The array to receive the bytes. If null, no bytes are returned</param>
        /// <param name="nvals">The number of bytes to read</param>
        /// <exception cref="IOException">Thrown if error reading input stream</exception>
        public void ReadByteBlockAlignedNoCrc(byte[] val, int nvals)
        {
            int destlength = nvals;
            while (nvals > 0)
            {
                int chunk = Math.Min(nvals, putByte - getByte);
                if (chunk == 0)
                {
                    ReadFromStream();
                }
                else
                {
                    if (val != null)
                        Array.Copy(buffer, getByte, val, destlength - nvals, chunk);

                    nvals -= chunk;
                    getByte += chunk;
                    availBits -= (chunk << BitsPerByteLog2);
                    totalBitsRead += (chunk << BitsPerByteLog2);
                }
            }
        }

        /// <summary>
        /// Read and count the number of zero bits.
        /// </summary>
        /// <returns>The number of zero bits read</returns>
        /// <exception cref="IOException">Thrown if error reading input stream</exception>
        public int ReadUnaryUnsigned()
        {
            int val = 0;
            int bit = ReadBit();
            while (bit == 0)
            {
                val++;
                bit = ReadBit();
            }
            return val;
        }

        /// <summary>
        /// Read a Rice Signal Block.
        /// </summary>
        /// <param name="vals">The values to be returned</param>
        /// <param name="pos">The starting position in the vals array</param>
        /// <param name="nvals">The number of values to return</param>
        /// <param name="parameter">The Rice parameter</param>
        /// <exception cref="IOException">Thrown if error reading input stream</exception>
        public void ReadRiceSignedBlock(int[] vals, int pos, int nvals, int parameter)
        {
            int j, valI = 0;
            int cbits = 0, uval = 0, msbs = 0, lsbsLeft = 0;
            byte blurb, saveBlurb;
            int state = 0; // 0 = getting unary MSBs, 1 = getting binary LSBs
            int i = getByte;

            long startBits = getByte * 8 + getBit;

            if (nvals == 0)
                return;

            // We unroll the main loop to take care of partially consumed blurbs here.
            if (getBit > 0)
            {
                saveBlurb = blurb = buffer[i];
                cbits = getBit;
                blurb <<= cbits;
                while (true)
                {
                    if (state == 0)
                    {
                        if (blurb != 0)
                        {
                            for (j = 0; (blurb & ByteTopBitOne) == 0; j++)
                                blurb <<= 1;
                            msbs += j;

                            // dispose of the unary end bit
                            blurb <<= 1;
                            j++;
                            cbits += j;
                            uval = 0;
                            lsbsLeft = parameter;
                            state++;
                            //totalBitsRead += msbs;
                            if (cbits == BitsPerByte)
                            {
                                cbits = 0;
                                readCrc16 = CRC16.Update(saveBlurb, readCrc16);
                                break;
                            }
                        }
                        else
                        {
                            msbs += BitsPerByte - cbits;
                            cbits = 0;
                            readCrc16 = CRC16.Update(saveBlurb, readCrc16);
                            //totalBitsRead += msbs;
                            break;
                        }
                    }
                    else
                    {
                        int availableBits = BitsPerByte - cbits;
                        if (lsbsLeft >= availableBits)
                        {
                            uval <<= availableBits;
                            uval |= ((blurb & 0xff) >> cbits);
                            cbits = 0;
                            readCrc16 = CRC16.Update(saveBlurb, readCrc16);
                            //totalBitsRead += availableBits;
                            if (lsbsLeft == availableBits)
                            {
                                // compose the value
                                uval |= (msbs << parameter);
                                if ((uval & 1) != 0)
                                    vals[pos + valI++] = -((int)(uval >> 1)) - 1;
                                else
                                    vals[pos + valI++] = (int)(uval >> 1);
                                if (valI == nvals)
                                    break;
                                msbs = 0;
                                state = 0;
                            }
                            lsbsLeft -= availableBits;
                            break;
                        }
                        else
                        {
                            uval <<= lsbsLeft;
                            uval |= ((blurb & 0xff) >> (BitsPerByte - lsbsLeft));
                            blurb <<= lsbsLeft;
                            cbits += lsbsLeft;
                            //totalBitsRead += lsbsLeft;
                            // compose the value
                            uval |= (msbs << parameter);
                            if ((uval & 1) != 0)
                                vals[pos + valI++] = -((int)(uval >> 1)) - 1;
                            else
                                vals[pos + valI++] = (int)(uval >> 1);
                            if (valI == nvals)
                            {
                                // back up one if we exited the for loop because we
                                // read all nvals but the end came in the middle of
                                // a blurb
                                i--;
                                break;
                            }
                            msbs = 0;
                            state = 0;
                        }
                    }
                }
                i++;
                getByte = i;
                getBit = cbits;
                //totalConsumedBits = (i << BITS_PER_BLURB_LOG2) | cbits;
                //totalBitsRead += (BITS_PER_BLURB) | cbits;
            }

            // Now that we are blurb-aligned the logic is slightly simpler
            while (valI < nvals)
            {
                for (; i < putByte && valI < nvals; i++)
                {
                    saveBlurb = blurb = buffer[i];
                    cbits = 0;
                    while (true)
                    {
                        if (state == 0)
                        {
                            if (blurb != 0)
                            {
                                for (j = 0; (blurb & ByteTopBitOne) == 0; j++)
                                    blurb <<= 1;
                                msbs += j;
                                // dispose of the unary end bit
                                blurb <<= 1;
                                j++;
                                cbits += j;
                                uval = 0;
                                lsbsLeft = parameter;
                                state++;
                                //totalBitsRead += msbs;
                                if (cbits == BitsPerByte)
                                {
                                    cbits = 0;
                                    readCrc16 = CRC16.Update(saveBlurb, readCrc16);
                                    break;
                                }
                            }
                            else
                            {
                                msbs += BitsPerByte - cbits;
                                cbits = 0;
                                readCrc16 = CRC16.Update(saveBlurb, readCrc16);
                                //totalBitsRead += msbs;
                                break;
                            }
                        }
                        else
                        {
                            int availableBits = BitsPerByte - cbits;
                            if (lsbsLeft >= availableBits)
                            {
                                uval <<= availableBits;
                                uval |= ((blurb & 0xff) >> cbits);
                                cbits = 0;
                                readCrc16 = CRC16.Update(saveBlurb, readCrc16);
                                //totalBitsRead += availableBits;
                                if (lsbsLeft == availableBits)
                                {
                                    // compose the value
                                    uval |= (msbs << parameter);
                                    if ((uval & 1) != 0)
                                        vals[pos + valI++] = -((int)(uval >> 1)) - 1;
                                    else
                                        vals[pos + valI++] = (int)(uval >> 1);
                                    if (valI == nvals)
                                        break;
                                    msbs = 0;
                                    state = 0;
                                }
                                lsbsLeft -= availableBits;
                                break;
                            }
                            else
                            {
                                uval <<= lsbsLeft;
                                uval |= ((blurb & 0xff) >> (BitsPerByte - lsbsLeft));
                                blurb <<= lsbsLeft;
                                cbits += lsbsLeft;
                                //totalBitsRead += lsbsLeft;
                                // compose the value
                                uval |= (msbs << parameter);
                                if ((uval & 1) != 0)
                                    vals[pos + valI++] = -((int)(uval >> 1)) - 1;
                                else
                                    vals[pos + valI++] = (int)(uval >> 1);
                                if (valI == nvals)
                                {
                                    // back up one if we exited the for loop because
                                    // we read all nvals but the end came in the
                                    // middle of a blurb
                                    i--;
                                    break;
                                }
                                msbs = 0;
                                state = 0;
                            }
                        }
                    }
                }
                getByte = i;
                getBit = cbits;
                //totalConsumedBits = (i << BITS_PER_BLURB_LOG2) | cbits;
                //totalBitsRead += (BITS_PER_BLURB) | cbits;
                if (valI < nvals)
                {
                    long endBits = getByte * 8 + getBit;
                    totalBitsRead += (int)(endBits - startBits);
                    availBits -= (int)(endBits - startBits);
                    ReadFromStream();
                    // these must be zero because we can only get here if we got to
                    // the end of the buffer
                    i = 0;
                    startBits = getByte * 8 + getBit;
                }
            }

            long endBits2 = getByte * 8 + getBit;
            totalBitsRead += (int)(endBits2 - startBits);
            availBits -= (int)(endBits2 - startBits);
        }

        /// <summary>
        /// Read UTF8 integer.
        /// On return, if *val == 0xffffffff then the utf-8 sequence was invalid, but
        /// the return value will be true.
        /// </summary>
        /// <param name="raw">The raw bytes read (output). If null, no bytes are returned</param>
        /// <param name="rawCount">The raw bytes count.</param>
        /// <returns>The integer read</returns>
        /// <exception cref="IOException">Thrown if error reading input stream</exception>
        public UInt32 ReadUtf8UInt32(byte[] raw, out int rawCount)
        {
            UInt32 val, x;
            UInt32 v = 0, i;
            
            x = ReadRawUInt32(8);
            rawCount = 0;

            if (raw != null) 
                raw[rawCount++] = (byte)x;

            if ((x & 0x80) == 0)
            { // 0xxxxxxx
                v = x;
                i = 0;
            }
            else if (((x & 0xC0) != 0) && ((x & 0x20) == 0))
            { // 110xxxxx
                v = x & 0x1F;
                i = 1;
            }
            else if (((x & 0xE0) != 0) && ((x & 0x10) == 0))
            { // 1110xxxx
                v = x & 0x0F;
                i = 2;
            }
            else if (((x & 0xF0) != 0) && ((x & 0x08) == 0))
            { // 11110xxx
                v = x & 0x07;
                i = 3;
            }
            else if (((x & 0xF8) != 0) && ((x & 0x04) == 0))
            { // 111110xx
                v = x & 0x03;
                i = 4;
            }
            else if (((x & 0xFC) != 0) && ((x & 0x02) == 0))
            { // 1111110x
                v = x & 0x01;
                i = 5;
            }
            else
            {
                val = 0xffffffff;
                return val;
            }
            for (; i > 0; i--)
            {
                x = PeekRawUInt32(8);
                if (((x & 0x80) == 0) || ((x & 0x40) != 0))
                { // 10xxxxxx
                    val = 0xffffffff;
                    return val;
                }
                x = ReadRawUInt32(8);
                if (raw != null)
                    raw[rawCount++] = (byte)x;

                v <<= 6;
                v |= (x & 0x3F);
            }
            val = v;
            return val;
        }

        /// <summary>
        /// Read UTF8 long.
        /// On return, if *val == 0xffffffffffffffff then the utf-8 sequence was invalid, but
        /// the return value will be true.
        /// </summary>
        /// <param name="raw">The raw bytes read (output). If null, no bytes are returned</param>
        /// <param name="rawCount">The raw bytes count.</param>
        /// <returns>The long read</returns>
        /// <exception cref="IOException">Thrown if error reading input stream</exception>
        public UInt64 ReadUtf8UInt64(byte[] raw, out int rawCount)
        {
            UInt64 val, v = 0;
            UInt32 x, i;

            x = ReadRawUInt32(8);
            rawCount = 0;

            if (raw != null)
                raw[rawCount++] = (byte)x;

            if (((x & 0x80) == 0))
            { // 0xxxxxxx
                v = x;
                i = 0;
            }
            else if (((x & 0xC0) != 0) && ((x & 0x20) == 0))
            { // 110xxxxx
                v = x & 0x1F;
                i = 1;
            }
            else if (((x & 0xE0) != 0) && ((x & 0x10) == 0))
            { // 1110xxxx
                v = x & 0x0F;
                i = 2;
            }
            else if (((x & 0xF0) != 0) && ((x & 0x08) == 0))
            { // 11110xxx
                v = x & 0x07;
                i = 3;
            }
            else if (((x & 0xF8) != 0) && ((x & 0x04) == 0))
            { // 111110xx
                v = x & 0x03;
                i = 4;
            }
            else if (((x & 0xFC) != 0) && ((x & 0x02) == 0))
            { // 1111110x
                v = x & 0x01;
                i = 5;
            }
            else if (((x & 0xFE) != 0) && ((x & 0x01) == 0))
            { // 11111110
                v = 0;
                i = 6;
            }
            else
            {
                val = 0xffffffffffffffffL;
                return val;
            }
            for (; i > 0; i--)
            {
                x = PeekRawUInt32(8);
                if (((x & 0x80) == 0) || ((x & 0x40) != 0))
                { // 10xxxxxx
                    val = 0xffffffffffffffffL;
                    return val;
                }
                x = ReadRawUInt32(8);

                if (raw != null)
                    raw[rawCount++] = (byte)x;
                v <<= 6;
                v |= (x & 0x3F);
            }

            val = v;
            return val;
        }

        /// <summary>
        /// Gets total bytes read.
        /// </summary>
        public int TotalBytesRead
        {
            get
            {
                return (totalBitsRead + 7) / 8;
            }
        }
    }
}
