using Flac.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flac.Frame
{
    /// <summary>
    /// FLAC constant subframe (channel) data.
    /// This class represents a FLAC subframe (channel) for a Constant data
    /// </summary>
    public class ChannelConstant : Channel
    {
        /// <summary>
        /// The constant signal value.
        /// </summary>
        private int value;

        /// <summary>
        /// Creates a new instance of ChannelConstant
        /// </summary>
        /// <param name="read">The bit reader</param>
        /// <param name="header">The FLAC Frame Header</param>
        /// <param name="channelData">The decoded channel data (output)</param>
        /// <param name="bps">The bits-per-second</param>
        /// <param name="wastedBits">The bits wasted in the frame</param>
        /// <exception cref="IOException">Thrown if error reading from BitReader</exception>
        public ChannelConstant(BitReader read, Header header, ChannelData channelData, int bps, int wastedBits)
            : base(header, wastedBits)
        {
            value = (int)read.ReadRawUInt32(bps);

            // Decode the subframe
            for (int i = 0; i < header.BlockSize; i++)
                channelData.Output[i] = value;
        }
    }
}
