using Flac.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flac
{
    /// <summary>
    /// FLAC channel data.
    /// This class holds the data for the channels in a FLAC frame.
    /// </summary>
    public class ChannelData
    {
        /// <summary>
        /// The output signal.
        /// </summary>
        public int[] Output
        {
            get;
            private set;
        }

        /// <summary>
        /// The risidual signal.
        /// </summary>
        public int[] Residual
        {
            get;
            private set;
        }

        /// <summary>
        /// The Entropy signal.
        /// </summary>
        public EntropyPartitionedRiceContents PartitionedRiceContents
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new instance of ChannelData
        /// </summary>
        /// <param name="size">The block size</param>
        public ChannelData(int size)
        {
            Output = new int[size];
            Residual = new int[size];
            PartitionedRiceContents = new EntropyPartitionedRiceContents();
        }
    }
}
