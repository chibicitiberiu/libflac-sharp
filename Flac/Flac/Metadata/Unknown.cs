using Flac.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flac.Metadata
{
    /// <summary>
    /// Unknown metadata block
    /// </summary>
    public class Unknown : Metadata
    {
        private byte[] data;

        /// <summary>
        /// Creates a new instance of Unknown.
        /// </summary>
        /// <param name="read">The BitReader</param>
        /// <param name="length">Length of the record</param>
        /// <param name="isLast">True if this is the last Metadata block in the chain</param>
        /// <exception cref="IOException">Thrown if error reading from BitReader</exception>
        public Unknown(BitReader read, int length, bool isLast)
            : base(isLast)
        {
            if (length > 0)
            {
                data = new byte[length];
                read.ReadByteBlockAlignedNoCrc(data, length);
            }
        }
    }
}
