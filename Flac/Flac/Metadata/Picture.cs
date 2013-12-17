using Flac.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flac.Metadata
{
    /// <summary>
    /// Picture Metadata block.
    /// </summary>
    public class Picture : Metadata
    {
        #region Constants
        private const int PictureTypeLength = 32;
        private const int MimeTypeByteCountLength = 32;
        private const int DescriptionByteCountLength = 32;
        private const int PicturePixelWidthLength = 32;
        private const int PicturePixelHeightLength = 32;
        private const int PictureBitsPerPixelLength = 32;
        private const int PictureColorCountLength = 32;
        private const int PictureByteCountLength = 32;
        #endregion

        private UInt32 pictureType;
        private string mimeType;
        private string description;
        private int picPixelWidth;
        private int picPixelHeight;
        private int picBitsPerPixel;
        private int picColorCount;
        private byte[] image;

        /// <summary>
        /// Creates a new instance of Picture.
        /// </summary>
        /// <param name="read">The BitReader</param>
        /// <param name="length">Length of the record</param>
        /// <param name="isLast">True if this is the last Metadata block in the chain</param>
        /// <exception cref="IOException">Thrown if error reading from BitReader</exception>
        public Picture(BitReader read, int length, bool isLast)
            : base(isLast)
        {
            int usedbits = 0;

            // Read picture type
            pictureType = read.ReadRawUInt32(PictureTypeLength);
            usedbits += PictureTypeLength;

            // Read mime type length
            int mimeTypeByteCount = (int)read.ReadRawUInt32(MimeTypeByteCountLength);
            usedbits += MimeTypeByteCountLength;

            // Read mime type
            byte[] data = new byte[mimeTypeByteCount];
            read.ReadByteBlockAlignedNoCrc(data, mimeTypeByteCount);
            usedbits += mimeTypeByteCount * 8;
            mimeType = UTF8Encoding.UTF8.GetString(data, 0, mimeTypeByteCount);

            // Description string length
            int descriptionByteCount = (int)read.ReadRawUInt32(DescriptionByteCountLength);
            usedbits += DescriptionByteCountLength;

            // Description string
            data = new byte[descriptionByteCount];
            read.ReadByteBlockAlignedNoCrc(data, descriptionByteCount);
            usedbits += descriptionByteCount * 8;
            description = UTF8Encoding.UTF8.GetString(data, 0, descriptionByteCount);

            // Picture stuff
            picPixelWidth = (int)read.ReadRawUInt32(PicturePixelWidthLength);
            picPixelHeight = (int)read.ReadRawUInt32(PicturePixelHeightLength);
            usedbits += PicturePixelWidthLength + PicturePixelHeightLength;
            
            picBitsPerPixel = (int)read.ReadRawUInt32(PictureBitsPerPixelLength);
            usedbits += PictureBitsPerPixelLength;

            picColorCount = (int)read.ReadRawUInt32(PictureColorCountLength);
            usedbits += PictureColorCountLength;

            // Read data size
            int picByteCount = (int)read.ReadRawUInt32(PictureByteCountLength);
            usedbits += PictureByteCountLength;
            
            // Read data
            image = new byte[picByteCount];
            read.ReadByteBlockAlignedNoCrc(image, picByteCount);
            usedbits += picByteCount * 8;

            // Skip the rest of the block
            length -= (usedbits / 8);
            read.ReadByteBlockAlignedNoCrc(null, length);
        }
    }
}
