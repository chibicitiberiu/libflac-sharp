using Flac.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flac.Metadata
{
    /// <summary>
    /// CueSheet Metadata block.
    /// </summary>
    public class CueSheet : Metadata
    {
        #region Constants
        private const int MediaCatalogNumberLength = 128 * 8;   // bits
        private const int LeadInLength = 64;                    // bits
        private const int IsCdLength = 1;                       // bits
        private const int ReservedLength = 7 + 258 * 8;         // bits
        private const int TrackCountLength = 8;                  // bits
        #endregion

        /// <summary>
        /// Media catalog number.
        /// in ASCII printable characters 0x20-0x7e.  In
        /// general, the media catalog number may be 0 to 128 bytes long; any
        /// unused characters should be right-padded with NUL characters.
        /// </summary>
        protected byte[] mediaCatalogNumber = new byte[129];

        /// <summary>
        /// The number of lead-in samples.
        /// </summary>
        protected UInt64 leadIn = 0;

        /// <summary>
        /// True if CueSheet corresponds to a Compact Disc, else false
        /// </summary>
        protected bool isCD = false;

        /// <summary>
        /// The number of tracks.
        /// </summary>
        protected UInt32 trackCount = 0;

        /// <summary>
        /// NULL if num_tracks == 0, else pointer to array of tracks.
        /// </summary>
        protected CueTrack[] tracks;

        /// <summary>
        /// Creates a new instance of CueSheet.
        /// </summary>
        /// <param name="read">The BitReader</param>
        /// <param name="length">Length of the record</param>
        /// <param name="isLast">True if this is the last Metadata block in the chain</param>
        /// <exception cref="IOException">Thrown if error reading from BitReader</exception>
        public CueSheet(BitReader read, int length, bool isLast)
            : base(isLast)
        {
            // Read fields
            read.ReadByteBlockAlignedNoCrc(mediaCatalogNumber, MediaCatalogNumberLength / 8);
            leadIn = read.ReadRawUInt64(LeadInLength);
            isCD = (read.ReadRawUInt32(IsCdLength) != 0);
            read.SkipBitsNoCrc(ReservedLength);
            trackCount = read.ReadRawUInt32(TrackCountLength);

            // Read cue tracks
            if (trackCount > 0)
            {
                tracks = new CueTrack[trackCount];
                for (int i = 0; i < trackCount; i++)
                    tracks[i] = new CueTrack(read);
            }
        }

        /// <summary>
        /// Verifies the Cue Sheet.
        /// </summary>
        /// <param name="checkCdDaSubset">True for check CD subset</param>
        /// <exception cref="ValidationException">Thrown if invalid Cue Sheet</exception>
        public void IsLegal(bool checkCdDaSubset)
        {

            if (checkCdDaSubset)
            {
                if (leadIn < 2 * 44100)
                    throw new ValidationException("CD-DA cue sheet must have a lead-in length of at least 2 seconds");
                
                if (leadIn % 588 != 0)
                    throw new ValidationException("CD-DA cue sheet lead-in length must be evenly divisible by 588 samples");
            }

            if (trackCount == 0)
                throw new ValidationException("cue sheet must have at least one track (the lead-out)");

            if (checkCdDaSubset && tracks[trackCount - 1].Number != 170)
                throw new ValidationException("CD-DA cue sheet must have a lead-out track number 170 (0xAA)");

            for (int i = 0; i < trackCount; i++)
            {
                if (tracks[i].Number == 0)
                    throw new ValidationException("cue sheet may not have a track number 0");

                if (checkCdDaSubset)
                {
                    if (!((tracks[i].Number >= 1 && tracks[i].Number <= 99)
                        || tracks[i].Number == 170))
                        throw new ValidationException("CD-DA cue sheet track number must be 1-99 or 170");
                }

                if (checkCdDaSubset && tracks[i].Offset % 588 != 0)
                    throw new ValidationException("CD-DA cue sheet track offset must be evenly divisible by 588 samples");

                if (i < trackCount - 1)
                {
                    if (tracks[i].IndicesCount == 0)
                        throw new ValidationException("cue sheet track must have at least one index point");

                    if (tracks[i].Indices[0].Number > 1)
                        throw new ValidationException("cue sheet track's first index number must be 0 or 1");
                }

                for (int j = 0; j < tracks[i].IndicesCount; j++)
                {
                    if (checkCdDaSubset && tracks[i].Indices[j].Offset % 588 != 0)
                        throw new ValidationException("CD-DA cue sheet track index offset must be evenly divisible by 588 samples");

                    if (j > 0)
                    {
                        if (tracks[i].Indices[j].Number != tracks[i].Indices[j - 1].Number + 1)
                            throw new ValidationException("cue sheet track index numbers must increase by 1");
                    }
                }
            }
        }
    }
}
