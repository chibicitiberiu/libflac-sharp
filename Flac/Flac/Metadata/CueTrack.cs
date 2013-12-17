using Flac.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flac.Metadata
{
    /// <summary>
    /// An entry into the cue sheet.
    /// </summary>
    public class CueTrack
    {
        #region Constants
        private const int OffsetLength = 64;
        private const int NumberLength = 8;
        private const int IsrcLength = 12 * 8;
        private const int TypeLength = 1;
        private const int PreEmphasisLength = 1;
        private const int ReservedLength = 6 + 13 * 8;
        private const int IndicesCountLength = 8;
        #endregion

        /// <summary>
        /// Track offset in samples, relative to the beginning of the FLAC audio stream.
        /// </summary>
        public UInt64 Offset { get; protected set; }

        /// <summary>
        /// The track number.
        /// </summary>
        public byte Number { get; protected set; }

        /// <summary>
        /// Track ISRC. This is a 12-digit alphanumeric code plus a trailing '\0'
        /// </summary>
        protected byte[] isrc = new byte[13];

        /// <summary>
        /// The track type: 0 for audio, 1 for non-audio.
        /// </summary>
        protected UInt32 type;

        /// <summary>
        /// The pre-emphasis flag: 0 for no pre-emphasis, 1 for pre-emphasis.
        /// </summary>
        protected UInt32 preEmphasis;

        /// <summary>
        /// The number of track index points.
        /// </summary>
        public byte IndicesCount { get; protected set; }

        /// <summary>
        /// NULL if num_indices == 0, else pointer to array of index points.
        /// </summary>
        public CueIndex[] Indices { get; protected set; }

        /// <summary>
        /// Creates new instance of CueTrack.
        /// </summary>
        /// <param name="read">The BitReader</param>
        /// <exception cref="IOException">Thrown if error reading from BitReader</exception>
        public CueTrack(BitReader read)
        {
            // Read fields
            Offset = read.ReadRawUInt64(OffsetLength);
            Number = (byte)read.ReadRawUInt32(NumberLength);
            read.ReadByteBlockAlignedNoCrc(isrc, IsrcLength / 8);
            type = read.ReadRawUInt32(TypeLength);
            preEmphasis = read.ReadRawUInt32(PreEmphasisLength);
            read.SkipBitsNoCrc(ReservedLength);
            IndicesCount = (byte)read.ReadRawUInt32(IndicesCountLength);

            // Read indices
            if (IndicesCount > 0)
            {
                Indices = new CueIndex[IndicesCount];

                for (int i = 0; i < IndicesCount; i++)
                    Indices[i] = new CueIndex(read);
            }
        }
    }
}
