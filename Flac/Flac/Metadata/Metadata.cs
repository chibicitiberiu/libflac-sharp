using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flac.Metadata
{
    /// <summary>
    /// Metadata types.
    /// </summary>
    public enum MetadataType
    {
        StreamInfo = 0,
        Padding = 1,
        Application = 2,
        SeekTable = 3,
        VorbisComment = 4,
        CueSheet = 5,
        Picture = 6,
        Undefined = 7
    }

    /// <summary>
    /// Root class for all Metadata subclasses.
    /// </summary>
    public abstract class Metadata
    {
        /// <summary>
        /// Metadata IsLast field length (bits).
        /// </summary>
        public const int StreamMetadataIsLastLength = 1;

        /// <summary>
        /// Metadata type field length (bits).
        /// </summary>
        public const int StreamMetadataTypeLength = 7;

        /// <summary>
        /// Metadata length field length (bits).
        /// </summary>
        public const int StreamMetadataLengthLength = 24;

        /// <summary>
        /// Test if this is the last metadata block.
        /// </summary>
        public bool IsLast { get; protected set; }

        /// <summary>
        /// Creates a new instance of Metadata class.
        /// </summary>
        /// <param name="isLast">True if this is the last metadata block.</param>
        public Metadata(bool isLast)
        {
            IsLast = isLast;
        }
    }
}
