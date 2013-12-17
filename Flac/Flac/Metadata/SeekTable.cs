using Flac.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flac.Metadata
{
    /// <summary>
    /// SeekTable metadata block.
    /// </summary>
    public class SeekTable : Metadata, IEnumerable<SeekPoint>
    {
        #region Constants
        private const int LengthBytes = 18;
        #endregion

        private SeekPoint[] points;

        /// <summary>
        /// Gets the seek point count.
        /// </summary>
        public int Length
        {
            get
            {
                if (points != null)
                    return points.Length;
                
                return 0;
            }
        }

        /// <summary>
        /// Gets length in bytes (metadata block size).
        /// </summary>
        public int RawLength
        {
            get
            {
                if (points != null)
                    return points.Length * LengthBytes;

                return 0;
            }
        }

        /// <summary>
        /// Gets iterator for this container
        /// </summary>
        /// <returns>The iterator</returns>
        public IEnumerator<SeekPoint> GetEnumerator()
        {
            foreach (var i in points)
                yield return i;
        }

        /// <summary>
        /// Gets iterator for this container
        /// </summary>
        /// <returns>The iterator</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return points.GetEnumerator();
        }

        /// <summary>
        /// Gets a seek point.
        /// </summary>
        /// <param name="index">Index of seek point.</param>
        /// <returns></returns>
        public SeekPoint this[int index]
        {
            get
            {
                return points[index];
            }
        }


        /// <summary>
        /// Creates a new instance of SeekTable.
        /// </summary>
        /// <param name="read">The BitReader</param>
        /// <param name="length">Length of the record</param>
        /// <param name="isLast">True if this is the last Metadata block in the chain</param>
        /// <exception cref="IOException">Thrown if error reading from BitReader</exception>
        public SeekTable(BitReader read, int length, bool isLast)
            : base(isLast)
        {
            // Calculate seek point count
            int pointCount = length / LengthBytes;

            // Create seek points
            points = new SeekPoint[pointCount];
            for (int i = 0; i < points.Length; i++)
                points[i] = new SeekPoint(read);

            // Skip rest of bytes
            length -= pointCount * LengthBytes;
            if (length > 0)
                read.ReadByteBlockAlignedNoCrc(null, length);
        }

        /// <summary>
        /// Creates a new instance of SeekTable.
        /// </summary>
        /// <param name="points">Seek points</param>
        /// <param name="isLast">True if this is the last Metadata block in the chain</param>
        public SeekTable(SeekPoint[] points, bool isLast)
            : base(isLast)
        {
            this.points = points;
        }

    }
}
