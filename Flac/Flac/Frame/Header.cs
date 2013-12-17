using Flac.IO;
using Flac.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flac.Frame
{
    /// <summary>
    /// Frame header class.
    /// </summary>
    public class Header
    {
        /// <summary>
        /// Gets or sets the number of samples per subframe.
        /// </summary>
        public int BlockSize { get; set; }

        /// <summary>
        /// Gets or sets the sample rate in Hz.
        /// </summary>
        public int SampleRate { get; set; }

        /// <summary>
        /// Gets or sets the number of channels (== number of subframes).
        /// </summary>
        public int Channels { get; set; }

        /// <summary>
        /// Gets or sets the channel assignment for the frame.
        /// </summary>
        public int ChannelAssignment { get; set; }

        /// <summary>
        /// Gets or sets the sample resolution.
        /// </summary>
        public int BitsPerSample { get; set; }

        /// <summary>
        /// Gets or sets the frame number or sample number of first sample in frame.
        /// Use the number_type value to determine which to use. 
        /// </summary>
        public int FrameNumber { get; set; }

        /// <summary>
        /// The sample number for the first sample in the frame.
        /// </summary>
        public int SampleNumber { get; set; }

        /// <summary>
        /// CRC-8 (polynomial = x^8 + x^2 + x^1 + x^0, initialized with 0)
        /// of the raw frame header bytes, meaning everything before the CRC byte
        /// including the sync code.
        /// </summary>
        protected byte crc;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="is"></param>
        /// <param name="headerWarmup"></param>
        /// <param name="streamInfo"></param>
        public Header(BitReader @is, byte[] headerWarmup, StreamInfo streamInfo)
        {
            FrameNumber = -1;
            SampleNumber = -1;
        }
    }
}
