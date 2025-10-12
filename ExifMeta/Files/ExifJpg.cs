// Based very loosely on the works of Hans-Peter Kalb

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ExifMeta;

namespace ExifMetaFile
{
    public static class ExifJpg
    {
        private enum ImageFileBlock
        {
            Unknown,
            Exif,
            Xmp,
            JpegComment,
            Icc,
            Sos,
            App0,
            App13Photoshop,
            App13,
            App14
        };

        private const int MaxExifBlockLen = 65534 - 2 - 6 - TiffHeaderConst.TiffHeaderLength;
        private const ushort App0Marker = 0xFFE0;
        private const ushort App1Marker = 0xFFE1;
        private const ushort App2Marker = 0xFFE2;
        private const ushort App13Marker = 0xFFED;
        private const ushort App14Marker = 0xFFEE;
        private const ushort CommentMarker = 0xFFFE; // Start of JPEG comment block
        private const ushort SosMarker = 0xFFDA; // Start of scan (SOS) marker
        private static readonly byte[] SoiMarker = new byte[] { 0xFF, 0xD8 }; // start of file
        private static readonly byte[] EoiMarker = new byte[] { 0xFF, 0xD9 }; // end of file
        private static readonly byte[] ExifSignature;
        private static readonly byte[] XmpReadSignature;
        private static readonly byte[] XmpWriteSignature;
        private static readonly byte[] App13ResourceSignature;
        private static readonly byte[] IccSignature;
        private const ushort IptcResourceId = 0x0404;
        private const ushort PhotoshopThumbnailResourceId = 0x0409;
        private const ushort IccProfileResourceId = 0x040F;
        private const ushort PathsResourceId = 0x0413;
        private const ushort LayerResourceId = 0x0422;
        private const ushort SlicingResourceId = 0x0424;

        static ExifJpg()
        {
            ExifSignature = Encoding.ASCII.GetBytes("Exif\0\0");
            XmpReadSignature = Encoding.ASCII.GetBytes("http://ns.adobe.com/xap/1.0/"); // no \0 at the end!
            XmpWriteSignature = Encoding.ASCII.GetBytes("http://ns.adobe.com/xap/1.0/\0");
            App13ResourceSignature = Encoding.ASCII.GetBytes("Photoshop 3.0\0");
            IccSignature = Encoding.ASCII.GetBytes("ICC_PROFILE\0");
        }

        public static bool IsJpgFile(Stream stream)
        {
            byte[] tempBuffer = new byte[SoiMarker.Length];

            stream.Position = 0;
            stream.Read(tempBuffer, 0, SoiMarker.Length);

            if (!ArrayHelper.CompareArrays(tempBuffer, 0, SoiMarker))
            {
                return false;
            }

            var count = stream.Length;
            stream.Position = count - EoiMarker.Length;
            stream.Read(tempBuffer, 0, EoiMarker.Length);

            if (!ArrayHelper.CompareArrays(tempBuffer, 0, EoiMarker))
            {
                return false;
            }

            return true;
        }

        public static ImageMetaData Load(Stream sourceStream)
        {
            ImageFileBlock blockType;
            int dataSize;
            Dictionary<int, byte[]> iccProfileSegments = new Dictionary<int, byte[]>();
            sourceStream.Position = 2;
            var imageMetaData = new ImageMetaData();
            imageMetaData.ImageType = ImageType.Jpeg;
            var exifReader = new ExifBinaryReader();

            do
            {
                var position = sourceStream.Position;
                ReadJpegBlock(sourceStream, out int blockContentSize, out _, out blockType, out dataSize);

                switch (blockType)
                {
                    case ImageFileBlock.Exif:
                    {
                        exifReader.ReadIfdWithTiffHeader(sourceStream, true, out ExifMetaData exifMetaData, out var _);
                        imageMetaData.ExifMetaData = exifMetaData;
                        break;
                    }

                    case ImageFileBlock.Xmp:
                    {
                        imageMetaData.Xmp = new byte[dataSize];
                        sourceStream.Read(imageMetaData.Xmp, 0, dataSize);

                        break;
                    }

                    case ImageFileBlock.App13Photoshop:
                    {
                        imageMetaData.Iptc = ExtractIptcFrom8BIMBlocks(sourceStream, dataSize);
                        break;
                    }

                    case ImageFileBlock.Icc:
                    {
                        var temp = new byte[2];
                        sourceStream.Read(temp, 0, 2);
                        int sequenceNr = (int)temp[0];
                        int maxBlocks = (int)temp[1];

                        dataSize -= 2;

                        imageMetaData.Icc = new byte[dataSize];
                        sourceStream.Read(imageMetaData.Icc, 0, dataSize);
                        iccProfileSegments.Add(sequenceNr, imageMetaData.Icc);

                        break;
                    }

                    case ImageFileBlock.JpegComment:
                    {
                        byte[] comment = new byte[dataSize];
                        sourceStream.Read(comment, 0, dataSize);
                        imageMetaData.Comment.SetRaw(comment);

                        break;
                    }
                }

                sourceStream.Position = position + blockContentSize + 4;
            }
            while (blockType != ImageFileBlock.Sos);

            imageMetaData.Icc = MergeIccSegments(iccProfileSegments);

            return imageMetaData;
        }

        private static byte[] MergeIccSegments(Dictionary<int, byte[]> iccProfileSegments)
        {
            if (iccProfileSegments.Count > 1)
            {
                int length = 0;
                int minSequenzNr = int.MaxValue;

                foreach (var segment in iccProfileSegments)
                {
                    if (segment.Key < minSequenzNr)
                    {
                        minSequenzNr = segment.Key;
                    }

                    length += segment.Value.Length;
                }

                var mergedIcc = new byte[length];
                int index = 0;
                int sequenzNr = minSequenzNr;

                for (int i = 0; i < iccProfileSegments.Count; i++)
                {
                    var segment = iccProfileSegments.First(x => x.Key == sequenzNr).Value;
                    Array.Copy(segment, 0, mergedIcc, index, segment.Length);
                    index += segment.Length;
                    sequenzNr++;
                }

                return mergedIcc;
            }
            else
            {
                return iccProfileSegments.First().Value;
            }
        }

        private static void ReadJpegBlockMarker(Stream imageStream, out ushort blockMarker, out int blockContentSize)
        {
            byte[] tempBuffer = new byte[2];

            imageStream.Read(tempBuffer, 0, 2);
            blockMarker = NumberConverter.BytesBigEndianToUInt16(tempBuffer);

            if (blockMarker == 0xFF01 || blockMarker >= 0xFFD0 && blockMarker <= 0xFFDA)
            {
                // Block does not have a size specification
                blockContentSize = 0;
            }
            else if (blockMarker == 0xFFFF)
            {
                throw new ExifException("Wrong file structure");
            }
            else
            {
                imageStream.Read(tempBuffer, 0, 2);
                blockContentSize = (int)NumberConverter.BytesBigEndianToUInt16(tempBuffer) - 2;

                if (blockContentSize < 0)
                {
                    throw new ExifException("Wrong file structure");
                }
            }
        }

        private static void ReadJpegBlock(Stream imageStream,
                                   out int blockContentSize,
                                   out ushort blockMarker,
                                   out ImageFileBlock blockType,
                                   out int dataSize)
        {
            blockType = ImageFileBlock.Unknown;
            ReadJpegBlockMarker(imageStream, out blockMarker, out blockContentSize);
            dataSize = blockContentSize;
            var position = imageStream.Position;

            switch (blockMarker)
            {
                case SosMarker:
                {
                    blockType = ImageFileBlock.Sos;
                    break;
                }

                case App0Marker:
                {
                    blockType = ImageFileBlock.App0;
                    break;
                }

                case App1Marker:
                {
                    int maxHeaderLength = Math.Max(ExifSignature.Length, XmpReadSignature.Length);
                    var header = new byte[maxHeaderLength];
                    imageStream.Read(header, 0, maxHeaderLength);

                    if (ArrayHelper.CompareArrays(header, 0, ExifSignature))
                    {
                        blockType = ImageFileBlock.Exif;
                        dataSize -= ExifSignature.Length;
                        imageStream.Position = position + ExifSignature.Length;
                    }
                    else
                    if (ArrayHelper.CompareArrays(header, 0, XmpReadSignature))
                    {
                        blockType = ImageFileBlock.Xmp;
                        int signatureLength = XmpReadSignature.Length + 1;

                        imageStream.Position = position + signatureLength - 1;
                        imageStream.Read(header, 0, 1);

                        while (header[0] != 0)
                        {
                            imageStream.Read(header, 0, 1);
                            signatureLength++;
                        }

                        dataSize -= signatureLength;
                    }
                    else
                    {
                        imageStream.Position = position;
                    }

                    break;
                }

                case App2Marker:
                {
                    var header = new byte[IccSignature.Length];
                    imageStream.Read(header, 0, IccSignature.Length);

                    if (ArrayHelper.CompareArrays(header, 0, IccSignature))
                    {
                        blockType = ImageFileBlock.Icc;
                        dataSize -= IccSignature.Length;
                    }
                    else
                    {
                        imageStream.Position = position;
                    }

                    break;
                }

                case App13Marker:
                {
                    var header = new byte[App13ResourceSignature.Length];
                    imageStream.Read(header, 0, App13ResourceSignature.Length);

                    if (ArrayHelper.CompareArrays(header, 0, App13ResourceSignature))
                    {
                        blockType = ImageFileBlock.App13Photoshop;
                        dataSize -= App13ResourceSignature.Length;
                    }
                    else
                    {
                        blockType = ImageFileBlock.App13;
                        imageStream.Position = position;
                    }

                    break;
                }

                case App14Marker:
                {
                    blockType = ImageFileBlock.App14;
                    break;
                }

                case CommentMarker:
                {
                    blockType = ImageFileBlock.JpegComment;
                    break;
                }
            }
        }

        /// <summary>
        /// Extract the IPTC data from a collection of 8BIM blocks.
        /// 
        /// The structure of an 8BIM block is:
        /// 
        /// Part          Length     Description
        /// ---------------------------------------------------------------------------------------
        /// signature     4 bytes    always 8BIM (ASCII), identifies the block type
        /// resource ID   2 bytes    the type of data that follows (e.g.. 0x0404 for IPTC)
        /// name          variable   Pascal string (1 byte length + content), often empty (0x00)
        /// padding	      0–1 byte   if length of the name is odd -> 1 byte padding
        /// data size     4 bytes    length of the following data in bytes
        /// data          variable   the data block (e.g. IPTC, thumbnail, etc.)
        /// padding	      0–1 byte   if length of the data is odd -> 1 byte padding
        /// </summary>
        /// <param name="imageStream">input file stream</param>
        /// <param name="blockSize">block size of the resources</param>
        /// <returns></returns>
        private static byte[] ExtractIptcFrom8BIMBlocks(Stream imageStream, int blockSize)
        {
            var data = new byte[blockSize];
            imageStream.Read(data, 0, blockSize);
            int offset = 0;

            while (offset + 12 < data.Length)
            {
                if (data[offset] != '8' || data[offset + 1] != 'B' || data[offset + 2] != 'I' || data[offset + 3] != 'M')
                {
                    break;
                }

                ushort resourceId = NumberConverter.BytesBigEndianToUInt16(data, offset + 4);
                offset += 6;

                byte nameLength = data[offset];
                offset += 1 + nameLength;

                if ((nameLength % 2) == 0)
                {
                    offset++;
                }

                uint dataSize = NumberConverter.BytesBigEndianToUInt32(data, offset);
                offset += 4;

                if (resourceId == IptcResourceId)
                {
                    var bytes = new byte[dataSize];
                    Array.Copy(data, offset, bytes, 0, dataSize);
                    return bytes;
                }

                offset += (int)dataSize;

                if ((dataSize % 2) == 1)
                {
                    offset++;
                }
            }

            return null;
        }

        public static void Save(Stream sourceStream, Stream destStream, ImageMetaDataWrite metadata)
        {
            const long firstBlockStartPosition = 2;
            ushort blockMarker;
            byte[] blockContent = new byte[65536];
            byte[] headerBuffer = new byte[4];

            sourceStream.Position = firstBlockStartPosition;
            destStream.Write(SoiMarker, 0, SoiMarker.Length);

            // Copy all APP0 blocks of the source file. These blocks should be the JFIF blocks.
            do
            {
                var position = sourceStream.Position;
                ReadJpegBlockMarker(sourceStream, out blockMarker, out int blockContentSize);

                if (blockMarker == SosMarker)
                {
                    break; // Start of JPEG image matrix reached
                }

                if (blockMarker == App0Marker)
                {
                    NumberConverter.UInt16ToBytesBigEndian(blockMarker, headerBuffer);
                    NumberConverter.UInt16ToBytesBigEndian((ushort)(blockContentSize + 2), headerBuffer, 2);
                    destStream.Write(headerBuffer, 0, 4);

                    if (sourceStream.Read(blockContent, 0, blockContentSize) != blockContentSize)
                    {
                        throw new ExifException("Wrong file structure");
                    }

                    destStream.Write(blockContent, 0, blockContentSize);
                }

                sourceStream.Position = position + blockContentSize + 4;
            }
            while (true);

            bool keepOriginalExif = SaveExif(sourceStream, destStream, metadata);
            bool keepOriginalXmp = SaveXmp(destStream, metadata);
            bool keepOriginalIcc = SaveIcc(destStream, metadata);
            bool keepOriginalIptc = SaveIptc(destStream, metadata);
            bool keepOriginalComment = SaveComment(destStream, metadata);

            sourceStream.Position = firstBlockStartPosition;
            bool stop = false;

            do
            {
                var position = sourceStream.Position;
                ReadJpegBlock(sourceStream, out int blockContentSize, out blockMarker, out ImageFileBlock blockType, out _);

                bool copyBlockFromSourceStream = true;

                switch (blockType)
                {
                    case ImageFileBlock.Sos:
                        // Start of JPEG image matrix reached. Copy all remaining data from source stream to destination stream
                        // without further interpretation.
                        NumberConverter.UInt16ToBytesBigEndian(blockMarker, headerBuffer);
                        destStream.Write(headerBuffer, 0, 2);
                        int bytesRead;

                        do
                        {
                            bytesRead = sourceStream.Read(blockContent, 0, blockContent.Length);
                            destStream.Write(blockContent, 0, bytesRead);
                        }
                        while (bytesRead == blockContent.Length);

                        copyBlockFromSourceStream = false;
                        stop = true;
                        break;

                    case ImageFileBlock.App0:
                        copyBlockFromSourceStream = false; // All APP0 blocks have already been copied
                        break;

                    case ImageFileBlock.Exif:
                        copyBlockFromSourceStream = keepOriginalExif;
                        break;

                    case ImageFileBlock.Xmp:
                        copyBlockFromSourceStream = keepOriginalXmp;
                        break;

                    case ImageFileBlock.JpegComment:
                        copyBlockFromSourceStream = keepOriginalComment;
                        break;

                    case ImageFileBlock.Icc:
                        copyBlockFromSourceStream = keepOriginalIcc;
                        break;

                    case ImageFileBlock.App13Photoshop:
                        copyBlockFromSourceStream = keepOriginalIptc;
                        break;

                    case ImageFileBlock.App13:
                        copyBlockFromSourceStream = metadata.App13Option == ImageMetaDataWriteOption.KeepOriginal;
                        break;

                    case ImageFileBlock.App14:
                        copyBlockFromSourceStream = metadata.App14Option == ImageMetaDataWriteOption.KeepOriginal;
                        break;
                }

                if (copyBlockFromSourceStream)
                {
                    NumberConverter.UInt16ToBytesBigEndian(blockMarker, headerBuffer);
                    NumberConverter.UInt16ToBytesBigEndian((ushort)(blockContentSize + 2), headerBuffer, 2);

                    sourceStream.Read(blockContent, 0, blockContentSize);
                    destStream.Write(headerBuffer, 0, 4);
                    destStream.Write(blockContent, 0, blockContentSize);
                }

                sourceStream.Position = position + blockContentSize + 4;
            }
            while (stop == false);
        }

        private static bool SaveExif(Stream sourceStream, Stream destStream, ImageMetaDataWrite imageMetaData)
        {
            bool keepOriginal = false;

            if (imageMetaData.ExifMetaDataOption == ImageMetaDataWriteOption.KeepOriginal)
            {
                keepOriginal = true;
            }
            else
            if (imageMetaData.ExifMetaDataOption == ImageMetaDataWriteOption.Remove)
            {
                keepOriginal = false;
            }
            else
            if (imageMetaData.ExifMetaDataOption == ImageMetaDataWriteOption.Overwrite && imageMetaData.ExifMetaData != null)
            {
                keepOriginal = false;

                var ifd = imageMetaData.ExifMetaData.ImageFileDirectories[0];

                var exifWriter = new ExifBinaryWriter(imageMetaData.ExifMetaData);
                var exifBinaryLen = exifWriter.GetSizeWithTiffHeader();

                if (exifBinaryLen > MaxExifBlockLen)
                {
                    throw new ExifException("Exif data too big");
                }

                if (exifBinaryLen > 0)
                {
                    byte[] headerBuffer = new byte[4];

                    NumberConverter.UInt16ToBytesBigEndian(App1Marker, headerBuffer);
                    NumberConverter.UInt16ToBytesBigEndian((ushort)(2 + ExifSignature.Length + exifBinaryLen), headerBuffer, 2);

                    destStream.Write(headerBuffer, 0, 4);
                    destStream.Write(ExifSignature, 0, ExifSignature.Length);

                    exifWriter.WriteAllWithTiffHeader(destStream);
                }
            }

            return keepOriginal;
        }

        private static bool SaveXmp(Stream destStream, ImageMetaDataWrite metadata)
        {
            bool keepOriginal = false;

            if (metadata.XmpOption == ImageMetaDataWriteOption.KeepOriginal)
            {
                keepOriginal = true;
            }
            else
            if (metadata.XmpOption == ImageMetaDataWriteOption.Remove)
            {
                keepOriginal = false;
            }
            else
            if (metadata.XmpOption == ImageMetaDataWriteOption.Overwrite && metadata.Xmp != null)
            {
                byte[] headerBuffer = new byte[4];

                keepOriginal = false;

                NumberConverter.UInt16ToBytesBigEndian(App1Marker, headerBuffer);
                NumberConverter.UInt16ToBytesBigEndian((ushort)(2 + XmpWriteSignature.Length + metadata.Xmp.Length), headerBuffer, 2);
                destStream.Write(headerBuffer, 0, 4);

                destStream.Write(XmpWriteSignature, 0, XmpWriteSignature.Length);
                destStream.Write(metadata.Xmp, 0, metadata.Xmp.Length);
            }

            return keepOriginal;
        }

        private static bool SaveIcc(Stream destStream, ImageMetaDataWrite metadata)
        {
            bool keepOriginal = false;

            if (metadata.IccOption == ImageMetaDataWriteOption.KeepOriginal)
            {
                keepOriginal = true;
            }
            else
            if (metadata.IccOption == ImageMetaDataWriteOption.Remove)
            {
                keepOriginal = false;
            }
            else
            if (metadata.IccOption == ImageMetaDataWriteOption.Overwrite && metadata.Icc != null)
            {
                byte[] headerBuffer = new byte[4];

                keepOriginal = false;

                int iccBlockOffset = 0;
                var iccLength = metadata.Icc.Length;
                var blockSize = 65534 - (2 + IccSignature.Length + 2);
                var numSegments = iccLength / blockSize;
                var blockSizeLastSegment = iccLength % blockSize;

                if (blockSizeLastSegment > 0)
                {
                    numSegments++;
                }

                for (int segment = 1; segment <= numSegments; segment++)
                {
                    if (segment == numSegments)
                    {
                        blockSize = blockSizeLastSegment;
                    }

                    // header (block marker, size)
                    NumberConverter.UInt16ToBytesBigEndian(App2Marker, headerBuffer);
                    NumberConverter.UInt16ToBytesBigEndian((ushort)(2 + IccSignature.Length + 2 + blockSize), headerBuffer, 2);
                    destStream.Write(headerBuffer, 0, 4);

                    // signature
                    destStream.Write(IccSignature, 0, IccSignature.Length);

                    headerBuffer[0] = (byte)segment;
                    headerBuffer[1] = (byte)numSegments;
                    destStream.Write(headerBuffer, 0, 2);

                    // data
                    destStream.Write(metadata.Icc, iccBlockOffset, blockSize);

                    iccBlockOffset += blockSize;
                }
            }

            return keepOriginal;
        }

        private static bool SaveIptc(Stream destStream, ImageMetaDataWrite metadata)
        {
            bool keepOriginal = false;

            if (metadata.IptcOption == ImageMetaDataWriteOption.KeepOriginal)
            {
                keepOriginal = true;
            }
            else
            if (metadata.IptcOption == ImageMetaDataWriteOption.Remove)
            {
                keepOriginal = false;
            }
            else
            if (metadata.IptcOption == ImageMetaDataWriteOption.Overwrite && metadata.Iptc != null)
            {
                byte[] headerBuffer = new byte[4];

                keepOriginal = false;

                // header (block marker, size)
                NumberConverter.UInt16ToBytesBigEndian(App13Marker, headerBuffer);
                var len8bim = 12 + metadata.Iptc.Length;

                if ((metadata.Iptc.Length & 1) != 0)
                {
                    len8bim++;
                }

                NumberConverter.UInt16ToBytesBigEndian((ushort)(2 + App13ResourceSignature.Length + len8bim), headerBuffer, 2);
                destStream.Write(headerBuffer, 0, 4);

                // signature
                destStream.Write(App13ResourceSignature, 0, App13ResourceSignature.Length);

                var bytes = Encoding.ASCII.GetBytes("8BIM");
                destStream.Write(bytes, 0, 4);

                NumberConverter.UInt16ToBytesBigEndian(IptcResourceId, bytes, 0);
                destStream.Write(bytes, 0, 2);

                destStream.WriteByte(0);
                destStream.WriteByte(0);

                NumberConverter.UInt32ToBytesBigEndian((uint)metadata.Iptc.Length, bytes, 0);
                destStream.Write(bytes, 0, 4);

                // data
                destStream.Write(metadata.Iptc, 0, metadata.Iptc.Length);

                if ((metadata.Iptc.Length % 2) == 1)
                {
                    destStream.WriteByte(0);
                }
            }

            return keepOriginal;
        }

        private static bool SaveComment(Stream destStream, ImageMetaDataWrite metadata)
        {
            bool keepOriginal = false;

            if (metadata.CommentOption == ImageMetaDataWriteOption.KeepOriginal)
            {
                keepOriginal = true;
            }
            else
            if (metadata.CommentOption == ImageMetaDataWriteOption.Remove)
            {
                keepOriginal = false;
            }
            else
            if (metadata.CommentOption == ImageMetaDataWriteOption.Overwrite && metadata.Comment != null)
            {
                byte[] headerBuffer = new byte[4];
                byte[] command = metadata.Comment.GetRaw();

                keepOriginal = false;

                // header (block marker, size)
                NumberConverter.UInt16ToBytesBigEndian(CommentMarker, headerBuffer);
                NumberConverter.UInt16ToBytesBigEndian((ushort)(2 + command.Length), headerBuffer, 2);
                destStream.Write(headerBuffer, 0, 4);

                // data
                destStream.Write(command, 0, command.Length);
            }

            return keepOriginal;
        }
    }
}
