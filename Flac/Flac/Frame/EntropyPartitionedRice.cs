using Flac.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flac.Frame
{
    /// <summary>
    /// This class holds the Entropy Partitioned Rice contents.
    /// </summary>
    public class EntropyPartitionedRice : EntropyCodingMethod
    {
        #region Constants
        private const int ParameterLength = 4;  // bits
        private const int RawLength = 5;        // bits
        private const UInt32 EscapeParameter = 15;
        #endregion

        /// <summary>
        /// The partition order, i.e. # of contexts = 2 ^ order.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// The context's Rice parameters and/or raw bits.
        /// </summary>
        public EntropyPartitionedRiceContents Contents
        {
            get;
            set;
        }

        /// <summary>
        /// Read compressed signal residual data.
        /// </summary>
        /// <param name="read">The bit reader</param>
        /// <param name="predictorOrder">The predicate order</param>
        /// <param name="partitionOrder">The partition order</param>
        /// <param name="header">The FLAC Frame Header</param>
        /// <param name="residual">The residual signal (output)</param>
        /// <exception cref="IOException">Thrown if error reading from BitReader</exception>
        public void ReadResidual(BitReader read, int predictorOrder, int partitionOrder, Header header, int[] residual)
        {
            int sample = 0;
            int partitions = 1 << partitionOrder;
            
            int partitionSamples;
            if (partitionOrder > 0)
                partitionSamples = (header.BlockSize >> partitionOrder);
            else
                partitionSamples = (header.BlockSize - predictorOrder);

            Contents = new EntropyPartitionedRiceContents();
            Contents.EnsureSize(Math.Max(6, partitionOrder));
            Contents.Parameters = new int[partitions]; // why allocate again?

            for (int partition = 0; partition < partitions; partition++)
            {
                int riceParameter = (int)read.ReadRawUInt32(ParameterLength);
                Contents.Parameters[partition] = riceParameter;

                if (riceParameter < EscapeParameter)
                {
                    int u = (partitionOrder == 0 || partition > 0) ?
                        (partitionSamples) :
                        (partitionSamples - predictorOrder);

                    read.ReadRiceSignedBlock(residual, sample, u, riceParameter);
                    sample += u;
                }

                else
                {
                    riceParameter = (int)read.ReadRawUInt32(RawLength);
                    Contents.RawBits[partition] = riceParameter;

                    int u = (partitionOrder == 0 || partition > 0) ? (0) : (predictorOrder);

                    for (; u < partitionSamples; u++, sample++)
                        residual[sample] = (int)read.ReadRawUInt32(riceParameter);
                }
            }
        }
    }
}
