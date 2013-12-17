using Flac.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flac.Metadata
{
    /// <summary>
    /// An entry into the seek table.
    /// </summary>
    public class SeekPoint
    {
        #region Constants
        private const int SampleNumberLength = 64;
        private const int StreamOffsetLength = 64;
        private const int FrameSamplesLength = 16;
        #endregion

        /// <summary>
        /// The sample number of the target frame.
        /// </summary>
        public long SampleNumber { get; private set; }

        /// <summary>
        /// The offset, in bytes, of the target frame with 
        /// respect to beginning of the first frame.
        /// </summary>
        public long StreamOffset { get; set; }

        /// <summary>
        /// The number of samples in the target frame.
        /// </summary>
        public int FrameSamples { get; private set; }

        /// <summary>
        /// Creates a new instance of SeekPoint.
        /// </summary>
        /// <param name="read">The BitReader</param>
        /// <exception cref="IOException">Thrown if error reading from BitReader</exception>
        public SeekPoint(BitReader read)
        {
            SampleNumber = (long)read.ReadRawUInt64(SampleNumberLength);
            StreamOffset = (long)read.ReadRawUInt64(StreamOffsetLength);
            FrameSamples = (int)read.ReadRawUInt32(FrameSamplesLength);
        }

        /// <summary>
        /// Creates a new instance of SeekPoint.
        /// </summary>
        /// <param name="sampleNumber">The sample number of the target frame</param>
        /// <param name="streamOffset">The offset, in bytes, of the target frame with 
        /// respect to beginning of the first frame</param>
        /// <param name="frameSamples">The number of samples in the target frame</param>
        public SeekPoint(long sampleNumber, long streamOffset, int frameSamples)
        {
            SampleNumber = sampleNumber;
            StreamOffset = streamOffset;
            FrameSamples = frameSamples;
        }
    }
}
