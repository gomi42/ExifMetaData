// Based loosely on the works of Hans-Peter Kalb

using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using ExifMeta;

namespace ExifMetaFile
{
    public static class ExifPng
    {
        private enum ImageFileBlock
        {
            Unknown,
            Exif,
            Iptc,
            Xmp,
            PngMetaData,
            PngDateChanged,
            Icc,
            Ihdr,
            Itxt,
            End
        };

        // PNG files
        private static readonly byte[] PngHeader = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        private static readonly byte[] PngIhdrChunk;
        private static readonly byte[] PngExifChunk;
        private static readonly byte[] PngItxtChunk;
        private static readonly byte[] PngTextChunk;
        private static readonly byte[] PngTimeChunk;
        private static readonly byte[] PngIccpChunk;
        private static readonly byte[] PngIendChunk;
        private static readonly byte[] PngXmpSignature;
        private static readonly byte[] PngIptcSignature;

        static ExifPng()
        {
            PngIhdrChunk = Encoding.ASCII.GetBytes("IHDR");
            PngExifChunk = Encoding.ASCII.GetBytes("eXIf");
            PngItxtChunk = Encoding.ASCII.GetBytes("iTXt");
            PngTextChunk = Encoding.ASCII.GetBytes("tEXt");
            PngTimeChunk = Encoding.ASCII.GetBytes("tIME");
            PngIccpChunk = Encoding.ASCII.GetBytes("iCCP");
            PngIendChunk = Encoding.ASCII.GetBytes("IEND");
            PngXmpSignature = Encoding.ASCII.GetBytes("XML:com.adobe.xmp\0");
            PngIptcSignature = Encoding.ASCII.GetBytes("Raw profile type iptc\0");

        }

        public static bool IsPngFile(Stream stream)
        {
            byte[] tempBuffer = new byte[PngHeader.Length];

            stream.Position = 0;
            stream.Read(tempBuffer, 0, PngHeader.Length);

            if (!ArrayHelper.CompareArrays(tempBuffer, 0, PngHeader))
            {
                return false;
            }

            return true;
        }

        // Read PNG file from stream "sourceStream".
        public static ImageMetaData Load(Stream sourceStream)
        {
            var imageMetaData = new ImageMetaData();
            imageMetaData.ImageType = ImageType.Png;
            sourceStream.Position = PngHeader.Length;
            ImageFileBlock chunkType;

            do
            {
                ReadBlockHeader(sourceStream, out chunkType, out int dataLength);
                var position = sourceStream.Position;

                switch (chunkType)
                {
                    case ImageFileBlock.Exif:
                    {
                        var exifReader = new ExifBinaryReader();
                        exifReader.ReadIfdWithTiffHeader(sourceStream, true, out ExifMetaData exifMetaData, out var _);
                        imageMetaData.ExifMetaData = exifMetaData;

                        break;
                    }

                    case ImageFileBlock.Itxt:
                    {
                        var chunkData = new byte[dataLength];
                        sourceStream.Read(chunkData, 0, dataLength);

                        var dataIndex = GetItxtSubHeaders(chunkData, out byte[][] headers);
                        var subChunk = GetItxtSubType(headers[0]);

                        if (subChunk == ImageFileBlock.Xmp)
                        {
                            imageMetaData.Xmp = new byte[dataLength - dataIndex];
                            Array.Copy(chunkData, dataIndex, imageMetaData.Xmp, 0, dataLength - dataIndex);
                        }

                        break;
                    }

                    case ImageFileBlock.Icc:
                    {
                        var chunkData = new byte[dataLength];
                        sourceStream.Read(chunkData, 0, dataLength);
                        imageMetaData.Icc = DecompressICCProfile(chunkData);

                        break;
                    }
                }

                sourceStream.Position = position + dataLength + 4;
            }
            while (chunkType != ImageFileBlock.End);

            return imageMetaData;
        }

        private static int GetItxtSubHeaders(byte[] chunkData, out byte[][] headers)
        {
            // Keyword\0CompressionFlag\0CompressionMethod\0LanguageTag\0TranslatedKeyword\0Text
            headers = new byte[5][];
            int segments = 0;
            int lastIndex = 0;
            int index = 0;

            while (segments != 5)
            {
                while (chunkData[index] != 0)
                {
                    index++;
                }

                var bytes = new byte[index - lastIndex + 1];
                Array.Copy(chunkData, lastIndex, bytes, 0, index - lastIndex + 1);
                headers[segments] = bytes;

                lastIndex = index + 1;
                index = lastIndex;
                segments++;
            }

            return lastIndex;
        }

        private static ImageFileBlock GetItxtSubType(byte[] keyword)
        {
            ImageFileBlock chunkType;

            if (ArrayHelper.CompareArrays(keyword, 0, PngXmpSignature))
            {
                chunkType = ImageFileBlock.Xmp;
            }
            else if (ArrayHelper.CompareArrays(keyword, 0, PngIptcSignature))
            {
                chunkType = ImageFileBlock.Iptc;
            }
            else
            {
                chunkType = ImageFileBlock.Unknown;
            }

            return chunkType;
        }

        private static void ReadBlockHeader(Stream imageStream, out ImageFileBlock chunkType, out int blockLength)
        {
            var header = new byte[8];

            if (imageStream.Read(header, 0, 8) < 8)
            {
                throw new ExifException("Wronge file structure");
            }

            blockLength = (int)NumberConverter.BytesBigEndianToUInt32(header);

            if (blockLength < 0)
            {
                throw new ExifException("Wronge file structure");
            }

            chunkType = ImageFileBlock.Unknown;

            if (ArrayHelper.CompareArrays(header, 4, PngExifChunk))
            {
                chunkType = ImageFileBlock.Exif;
            }
            else
            if (ArrayHelper.CompareArrays(header, 4, PngItxtChunk))
            {
                chunkType = ImageFileBlock.Itxt;
            }
            else
            if (ArrayHelper.CompareArrays(header, 4, PngTextChunk))
            {
                chunkType = ImageFileBlock.PngMetaData;
            }
            else
            if (ArrayHelper.CompareArrays(header, 4, PngTimeChunk))
            {
                chunkType = ImageFileBlock.PngDateChanged;
            }
            else
            if (ArrayHelper.CompareArrays(header, 4, PngIccpChunk))
            {
                chunkType = ImageFileBlock.Icc;
            }
            else
            if (ArrayHelper.CompareArrays(header, 4, PngIhdrChunk))
            {
                chunkType = ImageFileBlock.Ihdr;
            }
            else
            if (ArrayHelper.CompareArrays(header, 4, PngIendChunk))
            {
                chunkType = ImageFileBlock.End;
            }
        }

        public static void Save(Stream sourceStream, Stream destStream, ImageMetaDataWrite imageMetaData)
        {
            byte[] chunkData = new byte[65536];

            sourceStream.Position = PngHeader.Length;
            destStream.Write(PngHeader, 0, PngHeader.Length);
            ImageFileBlock chunkType;

            do
            {
                bool copyBlockFromSourceStream = true;

                long position = sourceStream.Position;
                ReadBlockHeader(sourceStream, out chunkType, out int dataLength);

                if (chunkType == ImageFileBlock.Exif && imageMetaData.ExifMetaDataOption != ImageMetaDataWriteOption.KeepOriginal)
                {
                    copyBlockFromSourceStream = false;
                }

                if (chunkType == ImageFileBlock.Itxt)
                {
                    chunkData = new byte[dataLength];
                    sourceStream.Read(chunkData, 0, dataLength);

                    var dataIndex = GetItxtSubHeaders(chunkData, out byte[][] headers);
                    var subChunk = GetItxtSubType(headers[0]);

                    if (subChunk == ImageFileBlock.Xmp && imageMetaData.XmpOption != ImageMetaDataWriteOption.KeepOriginal)
                    {
                        copyBlockFromSourceStream = false;
                    }
                }

                if (copyBlockFromSourceStream)
                {
                    // Copy current block from source to destination stream
                    sourceStream.Position = position;
                    int remainingByteCount = dataLength + 12;
                    int bytesToRead = chunkData.Length;

                    do
                    {
                        if (bytesToRead > remainingByteCount)
                        {
                            bytesToRead = remainingByteCount;
                        }

                        if (sourceStream.Read(chunkData, 0, bytesToRead) == bytesToRead)
                        {
                            destStream.Write(chunkData, 0, bytesToRead);
                            remainingByteCount -= bytesToRead;
                        }
                        else
                        {
                            throw new ExifException("Invalid file structure");
                        }

                    }
                    while (remainingByteCount > 0);
                }
                else
                {
                    sourceStream.Position = position + dataLength + 12;
                }

                if (chunkType == ImageFileBlock.Ihdr)
                {
                    // Write new EXIF block after the IHDR chunk if the new EXIF block is not empty
                    if (imageMetaData.ExifMetaDataOption == ImageMetaDataWriteOption.Overwrite
                        && imageMetaData.ExifMetaData != null
                        && !imageMetaData.ExifMetaData.IsEmpty())
                    {
                        var exifStream = new MemoryStream();
                        var exifWriter = new ExifBinaryWriter(imageMetaData.ExifMetaData);
                        exifWriter.WriteAllWithTiffHeader(exifStream);

                        var exifBinaryData = exifStream.ToArray();
                        int exifBinaryLen = exifBinaryData.Length;

                        var bytes4 = new byte[4];
                        NumberConverter.UInt32ToBytesBigEndian((uint)exifBinaryLen, bytes4);
                        destStream.Write(bytes4, 0, 4);

                        destStream.Write(PngExifChunk, 0, PngExifChunk.Length);
                        destStream.Write(exifBinaryData, 0, exifBinaryLen);

                        uint crc32 = CalculateCrc32(PngExifChunk, 0, PngExifChunk.Length, false);
                        crc32 = CalculateCrc32(exifBinaryData, 0, exifBinaryLen, true, crc32);
                        NumberConverter.UInt32ToBytesBigEndian(crc32, bytes4);
                        destStream.Write(bytes4, 0, 4);

                        exifStream.Dispose();
                    }

                    if (imageMetaData.XmpOption == ImageMetaDataWriteOption.Overwrite && imageMetaData.Xmp != null)
                    {
                        var itxtHeader = CreateItxtSubHeader(PngXmpSignature);

                        int blockLength = itxtHeader.Length + imageMetaData.Xmp.Length;
                        NumberConverter.UInt32ToBytesBigEndian((uint)blockLength, chunkData);
                        destStream.Write(chunkData, 0, 4);

                        destStream.Write(PngItxtChunk, 0, PngExifChunk.Length);
                        destStream.Write(itxtHeader, 0, itxtHeader.Length);
                        destStream.Write(imageMetaData.Xmp, 0, imageMetaData.Xmp.Length);

                        uint crc32 = CalculateCrc32(PngItxtChunk, 0, PngItxtChunk.Length, false);
                        crc32 = CalculateCrc32(itxtHeader, 0, itxtHeader.Length, false, crc32);
                        crc32 = CalculateCrc32(imageMetaData.Xmp, 0, imageMetaData.Xmp.Length, true, crc32);
                        NumberConverter.UInt32ToBytesBigEndian(crc32, chunkData);
                        destStream.Write(chunkData, 0, 4);
                    }
                }
            }
            while (chunkType != ImageFileBlock.End);
        }

        private static byte[] CreateItxtSubHeader(byte[] keyword)
        {
            // Keyword\0CompressionFlag\0CompressionMethod\0LanguageTag\0TranslatedKeyword\0Text

            var len = keyword.Length;
            var header = new byte[len + 4];
            Array.Copy(keyword, header, len);
            header[len] = 0;
            header[len + 1] = 0;
            header[len + 2] = 0;
            header[len + 3] = 0;
            return header;
        }

        // CRC32 checksum table for polynomial 0xEDB88320
        private static readonly uint[] Crc32ChecksumTable = { 0x00000000, 0x77073096, 0xEE0E612C, 0x990951BA, 0x076DC419, 0x706AF48F,
              0xE963A535, 0x9E6495A3, 0x0EDB8832, 0x79DCB8A4, 0xE0D5E91E, 0x97D2D988, 0x09B64C2B, 0x7EB17CBD, 0xE7B82D07, 0x90BF1D91,
              0x1DB71064, 0x6AB020F2, 0xF3B97148, 0x84BE41DE, 0x1ADAD47D, 0x6DDDE4EB, 0xF4D4B551, 0x83D385C7, 0x136C9856, 0x646BA8C0,
              0xFD62F97A, 0x8A65C9EC, 0x14015C4F, 0x63066CD9, 0xFA0F3D63, 0x8D080DF5, 0x3B6E20C8, 0x4C69105E, 0xD56041E4, 0xA2677172,
              0x3C03E4D1, 0x4B04D447, 0xD20D85FD, 0xA50AB56B, 0x35B5A8FA, 0x42B2986C, 0xDBBBC9D6, 0xACBCF940, 0x32D86CE3, 0x45DF5C75,
              0xDCD60DCF, 0xABD13D59, 0x26D930AC, 0x51DE003A, 0xC8D75180, 0xBFD06116, 0x21B4F4B5, 0x56B3C423, 0xCFBA9599, 0xB8BDA50F,
              0x2802B89E, 0x5F058808, 0xC60CD9B2, 0xB10BE924, 0x2F6F7C87, 0x58684C11, 0xC1611DAB, 0xB6662D3D, 0x76DC4190, 0x01DB7106,
              0x98D220BC, 0xEFD5102A, 0x71B18589, 0x06B6B51F, 0x9FBFE4A5, 0xE8B8D433, 0x7807C9A2, 0x0F00F934, 0x9609A88E, 0xE10E9818,
              0x7F6A0DBB, 0x086D3D2D, 0x91646C97, 0xE6635C01, 0x6B6B51F4, 0x1C6C6162, 0x856530D8, 0xF262004E, 0x6C0695ED, 0x1B01A57B,
              0x8208F4C1, 0xF50FC457, 0x65B0D9C6, 0x12B7E950, 0x8BBEB8EA, 0xFCB9887C, 0x62DD1DDF, 0x15DA2D49, 0x8CD37CF3, 0xFBD44C65,
              0x4DB26158, 0x3AB551CE, 0xA3BC0074, 0xD4BB30E2, 0x4ADFA541, 0x3DD895D7, 0xA4D1C46D, 0xD3D6F4FB, 0x4369E96A, 0x346ED9FC,
              0xAD678846, 0xDA60B8D0, 0x44042D73, 0x33031DE5, 0xAA0A4C5F, 0xDD0D7CC9, 0x5005713C, 0x270241AA, 0xBE0B1010, 0xC90C2086,
              0x5768B525, 0x206F85B3, 0xB966D409, 0xCE61E49F, 0x5EDEF90E, 0x29D9C998, 0xB0D09822, 0xC7D7A8B4, 0x59B33D17, 0x2EB40D81,
              0xB7BD5C3B, 0xC0BA6CAD, 0xEDB88320, 0x9ABFB3B6, 0x03B6E20C, 0x74B1D29A, 0xEAD54739, 0x9DD277AF, 0x04DB2615, 0x73DC1683,
              0xE3630B12, 0x94643B84, 0x0D6D6A3E, 0x7A6A5AA8, 0xE40ECF0B, 0x9309FF9D, 0x0A00AE27, 0x7D079EB1, 0xF00F9344, 0x8708A3D2,
              0x1E01F268, 0x6906C2FE, 0xF762575D, 0x806567CB, 0x196C3671, 0x6E6B06E7, 0xFED41B76, 0x89D32BE0, 0x10DA7A5A, 0x67DD4ACC,
              0xF9B9DF6F, 0x8EBEEFF9, 0x17B7BE43, 0x60B08ED5, 0xD6D6A3E8, 0xA1D1937E, 0x38D8C2C4, 0x4FDFF252, 0xD1BB67F1, 0xA6BC5767,
              0x3FB506DD, 0x48B2364B, 0xD80D2BDA, 0xAF0A1B4C, 0x36034AF6, 0x41047A60, 0xDF60EFC3, 0xA867DF55, 0x316E8EEF, 0x4669BE79,
              0xCB61B38C, 0xBC66831A, 0x256FD2A0, 0x5268E236, 0xCC0C7795, 0xBB0B4703, 0x220216B9, 0x5505262F, 0xC5BA3BBE, 0xB2BD0B28,
              0x2BB45A92, 0x5CB36A04, 0xC2D7FFA7, 0xB5D0CF31, 0x2CD99E8B, 0x5BDEAE1D, 0x9B64C2B0, 0xEC63F226, 0x756AA39C, 0x026D930A,
              0x9C0906A9, 0xEB0E363F, 0x72076785, 0x05005713, 0x95BF4A82, 0xE2B87A14, 0x7BB12BAE, 0x0CB61B38, 0x92D28E9B, 0xE5D5BE0D,
              0x7CDCEFB7, 0x0BDBDF21, 0x86D3D2D4, 0xF1D4E242, 0x68DDB3F8, 0x1FDA836E, 0x81BE16CD, 0xF6B9265B, 0x6FB077E1, 0x18B74777,
              0x88085AE6, 0xFF0F6A70, 0x66063BCA, 0x11010B5C, 0x8F659EFF, 0xF862AE69, 0x616BFFD3, 0x166CCF45, 0xA00AE278, 0xD70DD2EE,
              0x4E048354, 0x3903B3C2, 0xA7672661, 0xD06016F7, 0x4969474D, 0x3E6E77DB, 0xAED16A4A, 0xD9D65ADC, 0x40DF0B66, 0x37D83BF0,
              0xA9BCAE53, 0xDEBB9EC5, 0x47B2CF7F, 0x30B5FFE9, 0xBDBDF21C, 0xCABAC28A, 0x53B39330, 0x24B4A3A6, 0xBAD03605, 0xCDD70693,
              0x54DE5729, 0x23D967BF, 0xB3667A2E, 0xC4614AB8, 0x5D681B02, 0x2A6F2B94, 0xB40BBE37, 0xC30C8EA1, 0x5A05DF1B, 0x2D02EF8D };

        private static uint CalculateCrc32(byte[] data, int startIndex, int length, bool finalize = true, uint startCrc = 0xFFFFFFFF)
        {
            uint result = startCrc;
            int EndIndexPlus1 = startIndex + length;

            for (int i = startIndex; i < EndIndexPlus1; i++)
            {
                byte value = data[i];
                result = Crc32ChecksumTable[((byte)result) ^ value] ^ (result >> 8);
            }

            if (finalize)
                result = ~result;

            return result;
        }

        public static byte[] DecompressICCProfile(byte[] iccpChunkData)
        {
            // 1. Profilname extrahieren
            int nullIndex = Array.IndexOf(iccpChunkData, (byte)0);
            if (nullIndex < 0 || nullIndex + 2 >= iccpChunkData.Length)
                throw new InvalidDataException("Invalid iCCP-Chunk");

            // 2. Komprimierungsmethode prüfen
            byte compressionMethod = iccpChunkData[nullIndex + 1];
            if (compressionMethod != 0)
                throw new NotSupportedException("Only zlib compression supported");

            // 3. Komprimierte Daten extrahieren
            int compressedStart = nullIndex + 2;
            var compressedStream = new MemoryStream(iccpChunkData, compressedStart, iccpChunkData.Length - compressedStart);
            var deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress);
            var resultStream = new MemoryStream();
            deflateStream.CopyTo(resultStream);

            var resultBytes = resultStream.ToArray(); // Das entpackte ICC-Profil

            resultStream.Dispose();
            deflateStream.Dispose();
            compressedStream.Dispose();

            return resultBytes;
        }
    }
}
