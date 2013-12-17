using Flac.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flac.Metadata
{
    /// <summary>
    /// Application Metadata block.
    /// </summary>
    public class Application : Metadata
    {
        private const int IdLength = 32; // bits

        private byte[] id = new byte[4];
        private byte[] data;

        /// <summary>
        /// Creates a new instance of application class.
        /// </summary>
        /// <param name="read">The BitReader</param>
        /// <param name="length">Length of the record</param>
        /// <param name="isLast">True if this is the last Metadata block in the chain</param>
        /// <exception cref="IOException">Thrown if error reading from BitReader</exception>
        public Application(BitReader read, int length, bool isLast)
            : base(isLast)
        {
            read.ReadByteBlockAlignedNoCrc(id, IdLength / 8);
            length -= IdLength / 8;

            if (length > 0)
            {
                data = new byte[length];
                read.ReadByteBlockAlignedNoCrc(data, length);
            }
        }
    }
}
