using Flac.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Flac.Frame
{
    /// <summary>
    /// Fixed FLAC subframe (channel).
    /// </summary>
    public class ChannelFixed : Channel
    {
        private const int MaxFixedOrder = 4;

        /// <summary>
        /// The residual coding method.
        /// </summary>
        private Frame.EntropyCodingMethod entropyCodingMethod;

        /// <summary>
        /// The polynomial order.
        /// </summary>
        private int order;

        /// <summary>
        /// Warmup samples to prime the predictor, length == order.
        /// </summary>
        private int[] warmup = new int[MaxFixedOrder];

        /// <summary>
        /// The residual signal, length == (blocksize minus order) samples.
        /// </summary>
        private int[] residual;

        /// <summary>
        /// Creates a new instance of ChannelConstant
        /// </summary>
        /// <param name="read">The bit reader</param>
        /// <param name="header">The FLAC Frame Header</param>
        /// <param name="channelData">The decoded channel data (output)</param>
        /// <param name="bps">The bits-per-second</param>
        /// <param name="wastedBits">The bits wasted in the frame</param>
        /// <param name="order">The predicate order</param>
        /// <exception cref="IOException">Thrown if error reading from BitReader</exception>
        public ChannelFixed(BitReader read, Header header, ChannelData channelData, int bps, int wastedBits, int order)
            : base(header, wastedBits)
        {
            residual = channelData.Residual;
            this.order = order;

            // Read warmup samples
            for (int u = 0; u < order; u++)
                warmup[u] = (int)read.ReadRawUInt32(bps);

            // Read entropy coding method info
            int type = (int)read.ReadRawUInt32(TypeLength);
            EntropyPartitionedRice pr;

            switch (type)
            {
                case (int)EntropyCodingMethod.PartitionedRice:
                    pr = new EntropyPartitionedRice();
                    entropyCodingMethod = pr;
                    pr.Order = (int)read.ReadRawUInt32(PartitionedRiceOrderLength);
                    pr.Contents = channelData.PartitionedRiceContents;
                    pr.ReadResidual(read, order, pr.Order, header, channelData.Residual);
                    break;

                default:
                    throw new IOException("Unparseable stream!");
            }
        }
    }
}
