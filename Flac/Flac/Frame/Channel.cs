using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flac.Frame
{
    /// <summary>
    /// Base class for FLAC subframe (channel) classes.
    /// </summary>
    public abstract class Channel
    {
        #region Constants
        protected const int TypeLength = 2;
        protected const int PartitionedRiceOrderLength = 4;
        #endregion

        protected enum EntropyCodingMethod
        {
            PartitionedRice = 0
        }

        /// <summary>
        /// The FLAC Frame Header.
        /// </summary>
        protected Header Header { get; set; }

        /// <summary>
        /// The number of wasted bits in the frame.
        /// </summary>
        public int WastedBits { get; protected set; }

        /// <summary>
        /// Creates a new instance of channel.
        /// </summary>
        /// <param name="header">The FLAC Frame Header.</param>
        /// <param name="wastedBits">The number of wasted bits in the frame.</param>
        protected Channel(Header header, int wastedBits)
        {
            Header = header;
            WastedBits = wastedBits;
        }
    }
}
