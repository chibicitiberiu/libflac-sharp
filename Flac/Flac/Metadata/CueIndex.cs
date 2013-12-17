using Flac.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flac.Metadata
{
    /// <summary>
    /// An entry into the cue track.
    /// </summary>
    public class CueIndex
    {
        private const int OffsetLength = 64;    // bits
        private const int NumberLength = 8;     // bits
        private const int ReservedLength = 3 * 8; // bits

        /// <summary>
        /// Offset in samples, relative to the track offset, of the index point.
        /// </summary>
        public UInt64 Offset { get; protected set; }

        /// <summary>
        /// The index point number.
        /// </summary>
        public byte Number { get; protected set; }

        /// <summary>
        /// Creates a new instance of CueIndex.
        /// </summary>
        /// <param name="read">The BitReader</param>
        /// <exception cref="IOException">Thrown if error reading from BitReader.</exception>
        public CueIndex(BitReader read)
        {
            Offset = read.ReadRawUInt64(OffsetLength);
            Number = (byte)read.ReadRawUInt32(NumberLength);
            read.SkipBitsNoCrc(ReservedLength);
        }
    }
}
