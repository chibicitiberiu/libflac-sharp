using Flac.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flac.Metadata
{
    /// <summary>
    /// StreamInfo Metadata block.
    /// </summary>
    public class StreamInfo : Metadata
    {
        #region Constants
        private const int MinBlockSizeLength = 16;  // bits
        private const int MaxBlockSizeLength = 16;  // bits
        private const int MinFrameSizeLength = 24;  // bits
        private const int MaxFrameSizeLength = 24;  // bits
        private const int SampleRateLength = 20;    // bits
        private const int ChannelsLength = 3;       // bits
        private const int BitsPerSampleLength = 5;  // bits
        private const int TotalSamplesLength = 36;  // bits
        private const int Md5SumLength = 128;       // bits
        #endregion

        private byte[] md5Sum = new byte[16];

        /// <summary>
        /// Gets the MinBlockSize
        /// </summary>
        public UInt32 MinBlockSize { get; private set; }

        /// <summary>
        /// Gets the MaxBlockSize
        /// </summary>
        public UInt32 MaxBlockSize { get; private set; }

        /// <summary>
        /// Gets the MinFrameSize
        /// </summary>
        public UInt32 MinFrameSize { get; private set; }

        /// <summary>
        /// Gets the MaxFrameSize
        /// </summary>
        public UInt32 MaxFrameSize { get; private set; }

        /// <summary>
        /// Gets the sample rate.
        /// </summary>
        public UInt32 SampleRate { get; private set; }

        /// <summary>
        /// Gets the number of channels.
        /// </summary>
        public UInt32 Channels { get; private set; }

        /// <summary>
        /// Gets the number of bits per sample.
        /// </summary>
        public UInt32 BitsPerSample { get; private set; }

        /// <summary>
        /// Gets or sets the total number of samples.
        /// </summary>
        public UInt64 TotalSamples { get; set; }
        
        /// <summary>
        /// Creates a new instance of stream info class.
        /// </summary>
        /// <param name="read">The BitReader</param>
        /// <param name="length">Length of the record</param>
        /// <param name="isLast">True if this is the last Metadata block in the chain.</param>
        /// <exception cref="IOException">Thrown if error reading from InputBitStream</exception>
        public StreamInfo(BitReader read, int length, bool isLast)
            : base(isLast)
        {
            int usedBits = 0;

            MinBlockSize = read.ReadRawUInt32(MinBlockSizeLength);
            usedBits += MinBlockSizeLength;

            MaxBlockSize = read.ReadRawUInt32(MaxBlockSizeLength);
            usedBits += MaxBlockSizeLength;

            MinFrameSize = read.ReadRawUInt32(MinFrameSizeLength);
            usedBits += MinFrameSizeLength;

            MaxFrameSize = read.ReadRawUInt32(MaxFrameSizeLength);
            usedBits += MaxFrameSizeLength;

            SampleRate = read.ReadRawUInt32(SampleRateLength);
            usedBits += SampleRateLength;

            Channels = read.ReadRawUInt32(ChannelsLength) + 1;
            usedBits += ChannelsLength;

            BitsPerSample = read.ReadRawUInt32(BitsPerSampleLength) + 1;
            usedBits += BitsPerSampleLength;

            TotalSamples = read.ReadRawUInt64(TotalSamplesLength);
            usedBits += TotalSamplesLength;

            read.ReadByteBlockAlignedNoCrc(md5Sum, Md5SumLength / 8);
            usedBits += Md5SumLength;

            // Skip the rest of the block
            length -= (usedBits / 8);
            read.ReadByteBlockAlignedNoCrc(null, length);
        }

        /// <summary>
        /// Gets the metadata block size.
        /// </summary>
        public int Length
        {
            get
            {
                int bits = MinBlockSizeLength
                    + MaxBlockSizeLength
                    + MinFrameSizeLength
                    + MaxFrameSizeLength
                    + SampleRateLength
                    + ChannelsLength
                    + BitsPerSampleLength
                    + TotalSamplesLength
                    + Md5SumLength;

                return (bits + 7) / 8;
            }
        }

        /// <summary>
        /// Checks for compatible StreamInfo.
        /// Checks if SampleRate, Channels, and BitsPerSample are equal
        /// </summary>
        /// <param name="info">The StreamInfo block to check</param>
        /// <returns>True if this and info are compatable</returns>
        public bool Compatible (StreamInfo info)
        {
            if (SampleRate != info.SampleRate) return false;
            if (Channels != info.Channels) return false;
            if (BitsPerSample != info.BitsPerSample) return false;
            return true;
        }
    }
}
