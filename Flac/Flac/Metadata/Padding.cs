using Flac.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flac.Metadata
{
    /// <summary>
    /// Padding Metadata block.
    /// </summary>
    public class Padding : Metadata
    {
        private int length;

        /// <summary>
        /// Creates a new instance of Padding.
        /// </summary>
        /// <param name="read">The BitReader</param>
        /// <param name="length">Length of the record</param>
        /// <param name="isLast">True if this is the last Metadata block in the chain</param>
        /// <exception cref="IOException">Thrown if error reading from BitReader</exception>
        public Padding(BitReader read, int length, bool isLast)
            : base(isLast)
        {
            this.length = length;
            read.ReadByteBlockAlignedNoCrc(null, length);
        }
    }
}
