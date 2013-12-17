using Flac.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flac.Metadata
{
    /// <summary>
    /// Vorbis comment metadata block.
    /// </summary>
    public class VorbisComment : Metadata, IEnumerable<KeyValuePair<string, string>>
    {
        private string vendor;
        public Dictionary<string, string> Comments { get; private set; }

        /// <summary>
        /// Creates a new instance of VorbisComment.
        /// </summary>
        /// <param name="read">The BitReader</param>
        /// <param name="length">Length of the record</param>
        /// <param name="isLast">True if this is the last Metadata block in the chain</param>
        /// <exception cref="IOException">Thrown if error reading from BitReader</exception>
        public VorbisComment(BitReader read, int length, bool isLast)
            : base(isLast)
        {
            byte[] data;

            // Read vendor string
            int vendorLength = (int)read.ReadRawUInt32LittleEndian();
            data = new byte[vendorLength];
            read.ReadByteBlockAlignedNoCrc(data, vendorLength);

            vendor = UTF8Encoding.UTF8.GetString(data, 0, vendorLength);

            // Read comments
            int commentsCount = (int)read.ReadRawUInt32LittleEndian();
            Comments = new Dictionary<string, string>(commentsCount);

            for (int i = 0; i < commentsCount; i++)
            {
                // Get comment bytes
                int len = (int)read.ReadRawUInt32LittleEndian();
                data = new byte[len];
                read.ReadByteBlockAlignedNoCrc(data, len);

                // Obtain string
                string str = UTF8Encoding.UTF8.GetString(data, 0, len);
                int equal = str.IndexOf('=');

                // Add comment
                if (equal > 0)
                    this.Comments.Add(str.Substring(0, equal), str.Substring(equal));
            }
        }

        /// <summary>
        /// Gets iterator for this container
        /// </summary>
        /// <returns>The iterator</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return Comments.GetEnumerator();
        }
        
        /// <summary>
        /// Gets iterator for this container
        /// </summary>
        /// <returns>The iterator</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)Comments).GetEnumerator();
        }
    }
}
