using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ExifMeta
{
    public class ExifBinaryReader
    {
        private const int MaxTagValueCount = int.MaxValue / 8;

        internal Stream sourceReader;
        internal uint exifFileOffset;
        private ByteOrder byteOrder = ByteOrder.LittleEndian;

        public ExifBinaryReader()
        {
        }

        public ByteOrder ByteOrder => ByteOrder;

        public void ReadIfd(Stream fileStream, uint fileOffset, uint ifdOffset, out ExifMetaData exifMeta, out uint nextOffset)
        {
            sourceReader = fileStream;
            exifFileOffset = fileOffset;

            exifMeta = new ExifMetaData();
            exifMeta.fileOffset = fileOffset;

            nextOffset = ReadIfd(ifdOffset, out Ifd ifd);
            exifMeta.ImageFileDirectories.Add(ifd);
        }

        public ExifMetaData Read(Stream fileStream)
        {
            sourceReader = fileStream;
            exifFileOffset = (uint)fileStream.Position;

            ReadTiffHeader(fileStream, out ByteOrder byteOrder, out uint next);

            this.byteOrder = byteOrder;

            var exifMeta = new ExifMetaData();
            exifMeta.fileOffset = exifFileOffset;

            while (next != 0)
            {
                next = ReadIfd(next, out Ifd ifd);
                exifMeta.ImageFileDirectories.Add(ifd);
            }

            return exifMeta;
        }

        private void ReadTiffHeader(Stream fileStream, out ByteOrder byteOrder, out uint ifdOffset)
        {
            byte[] header = new byte[TiffHeaderConst.TiffHeaderLength];
            int tiffHeaderBytesRead = fileStream.Read(header, 0, TiffHeaderConst.TiffHeaderLength);

            if (tiffHeaderBytesRead < TiffHeaderConst.TiffHeaderLength)
            {
                throw new ExifException("Wrong file structure");
            }

            if (ArrayHelper.CompareArrays(header, 0, TiffHeaderConst.TiffHeaderSignatureLittleEndian))
            {
                byteOrder = ByteOrder.LittleEndian;
            }
            else
            if (ArrayHelper.CompareArrays(header, 0, TiffHeaderConst.TiffHeaderSignatureBigEndian))
            {
                byteOrder = ByteOrder.BigEndian;
            }
            else
            {
                throw new ExifException("Wrong file structure");
            }

            ifdOffset = NumberConverter.BytesToUInt32(header, 4, byteOrder);

            if (ifdOffset < TiffHeaderConst.TiffHeaderLength)
            {
                throw new ExifException("Wrong file structure");
            }
        }

        private uint ReadIfd(uint ifdOffset, out Ifd ifd)
        {
            ifd = new Ifd();
            return ReadSimpleIfd(ifdOffset, ifd);
        }

        private void ReadTagData(byte[] ifdBytes, int tagIndex, out DataType dataType, out int valueCount, out byte[] tagData)
        {
            dataType = (DataType)NumberConverter.BytesToUInt16(ifdBytes, tagIndex + 2, byteOrder);
            valueCount = (int)NumberConverter.BytesToUInt32(ifdBytes, tagIndex + 4, byteOrder);

            if (valueCount > MaxTagValueCount)
            {
                throw new ExifException("Wrong file structure");
            }

            var valueByteCount = TagHelper.GetTagByteCount(dataType, valueCount);

            if (valueByteCount <= 4)
            {
                tagData = new byte[valueByteCount];
                Array.Copy(ifdBytes, tagIndex + 8, tagData, 0, valueByteCount);
            }
            else
            {
                var originalOffset = NumberConverter.BytesToUInt32(ifdBytes, tagIndex + 8, byteOrder);
                tagData = ReadBytesFromSource(originalOffset, valueByteCount);
            }
        }

        private void ReadTagDataOffset(byte[] ifdBytes, int tagIndex, out DataType dataType, out int valueCount, out uint dataOffset)
        {
            dataType = (DataType)NumberConverter.BytesToUInt16(ifdBytes, tagIndex + 2, byteOrder);
            valueCount = (int)NumberConverter.BytesToUInt32(ifdBytes, tagIndex + 4, byteOrder);

            if (valueCount > MaxTagValueCount)
            {
                throw new ExifException("Wrong file structure");
            }

            var valueByteCount = TagHelper.GetTagByteCount(dataType, valueCount);

            if (valueByteCount <= 4)
            {
                dataOffset = (uint)(tagIndex + 8);
            }
            else
            {
                dataOffset = NumberConverter.BytesToUInt32(ifdBytes, tagIndex + 8, byteOrder);
            }
        }

        private Property CreateProperty(IfdId ifdId, TagId tagId, TagDetails tagDetails, byte[] ifdBytes, int tagIndex)
        {
            if (tagDetails.DontLoadPayload)
            {
                ReadTagDataOffset(ifdBytes, tagIndex, out var dataType, out var valueCount, out var dataOffset);
                return CallFromStream(tagDetails.Property, tagId, dataType, sourceReader, exifFileOffset + dataOffset, valueCount);
            }
            else
            {
                ReadTagData(ifdBytes, tagIndex, out var dataType, out var valueCount, out var dataBytes);
                return CallFromBinary(tagDetails.Property, tagId, dataType, valueCount, dataBytes, byteOrder);
            }
        }

        public Property CallFromStream(Type propertyType,
                                       TagId tagId,
                                       DataType dataType,
                                       Stream sourceStream,
                                       long sourcePosition,
                                       int count)
        {
            while (propertyType != typeof(Property) && propertyType != typeof(object))
            {
                Type searchType;

                if (propertyType.IsGenericType)
                {
                    searchType = propertyType.GetGenericTypeDefinition().MakeGenericType(propertyType.GetGenericArguments());
                }
                else
                {
                    searchType = propertyType;
                }

                var method = propertyType.GetMethod(
                                "FromStream",
                                BindingFlags.Static | BindingFlags.NonPublic,
                                null,
                                new Type[] { typeof(TagId), typeof(DataType), typeof(Stream), typeof(long), typeof(int) },
                                null);

                if (method != null)
                {
                    return (Property)method.Invoke(null, new object[] { tagId, dataType, sourceStream, sourcePosition, count });
                }

                propertyType = propertyType.BaseType;
            }

            throw new MissingMethodException("'FromStream' method not found.");
        }

        public Property CallFromBinary(Type propertyType,
                                       TagId tagId,
                                       DataType dataType,
                                       int count,
                                       byte[] bytes,
                                       ByteOrder byteOrder)
        {
            while (propertyType != typeof(Property) && propertyType != typeof(object))
            {
                Type searchType;

                if (propertyType.IsGenericType)
                {
                    searchType = propertyType.GetGenericTypeDefinition().MakeGenericType(propertyType.GetGenericArguments());
                }
                else
                {
                    searchType = propertyType;
                }

                var method = searchType.GetMethod(
                                "FromBinary",
                                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public,
                                null,
                                new Type[] { typeof(TagId), typeof(DataType), typeof(int), typeof(byte[]), typeof(ByteOrder) },
                                null);

                if (method != null)
                {
                    return (Property)method.Invoke(null, new object[] { tagId, dataType, count, bytes, byteOrder });
                }

                method = searchType.GetMethod(
                                "FromBinary",
                                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public,
                                null,
                                new Type[] { typeof(TagId), typeof(int), typeof(byte[]), typeof(ByteOrder) },
                                null);

                if (method != null)
                {
                    return (Property)method.Invoke(null, new object[] { tagId, count, bytes, byteOrder });
                }

                propertyType = propertyType.BaseType;
            }

            throw new MissingMethodException("'FromBinary' method not found.");
        }

        private void ReadSubIfdsIfd(List<List<Ifd>> subIfdsIfd, byte[] ifdBytes, int ifdBytesEntryIndex)
        {
            ReadTagData(ifdBytes, ifdBytesEntryIndex, out _, out var valueCount, out var tagData);

            for (int i = 0; i < valueCount; i++)
            {
                var ifdChain = new List<Ifd>();
                subIfdsIfd.Add(ifdChain);

                var next = NumberConverter.BytesToUInt32(tagData, i * 4, byteOrder);

                while (next != 0)
                {
                    next = ReadIfd(next, out Ifd ifd);
                    ifdChain.Add(ifd);
                }
            }
        }

        private uint ReadSimpleIfd(uint ifdOffset, Ifd destIfd)
        {
            byte[] tagCountArray = ReadBytesFromSource(ifdOffset, 2);
            var tagCount = NumberConverter.BytesToUInt16(tagCountArray, 0, byteOrder);

            int ifdSize = tagCount * 12 + 4;
            var ifdBytes = ReadBytesFromSource(ifdOffset + 2, ifdSize);

            var nextIfdBytesEntryIndex = 0;

            for (int j = 0; j < tagCount; j++)
            {
                var ifdBytesEntryIndex = nextIfdBytesEntryIndex;
                nextIfdBytesEntryIndex += 12;

                Tag tag = (Tag)NumberConverter.BytesToUInt16(ifdBytes, ifdBytesEntryIndex, byteOrder);
                var tagId = TagDetailsRegistry.FindTagId(destIfd.IfdId, tag, out var tagDetails);

                if (tagDetails == null)
                {
                    continue;
                }

                switch (tagId)
                {
                    case TagId.ExifIfdPointer:
                        var exifSubIfdOffset = NumberConverter.BytesToUInt32(ifdBytes, ifdBytesEntryIndex + 8, byteOrder);
                        ReadSimpleIfd(exifSubIfdOffset, destIfd.ExifIfd);
                        break;

                    case TagId.GpsIfdPointer:
                        var gpsSubIfdOffset = NumberConverter.BytesToUInt32(ifdBytes, ifdBytesEntryIndex + 8, byteOrder);
                        ReadSimpleIfd(gpsSubIfdOffset, destIfd.GpsIfd);
                        break;

                    case TagId.InteropIfdPointer:
                        var interopIfdOffset = NumberConverter.BytesToUInt32(ifdBytes, ifdBytesEntryIndex + 8, byteOrder);
                        ReadSimpleIfd(interopIfdOffset, destIfd.InteropIfd);
                        break;

                    case TagId.GlobalParametersIFD:
                        var globalParamsIfdOffset = NumberConverter.BytesToUInt32(ifdBytes, ifdBytesEntryIndex + 8, byteOrder);
                        ReadSimpleIfd(globalParamsIfdOffset, destIfd.GlobalParamsIfd);
                        break;

                    case TagId.SubIFDs:
                        var subIfdOffset = NumberConverter.BytesToUInt32(ifdBytes, ifdBytesEntryIndex + 8, byteOrder);
                        ReadSubIfdsIfd(destIfd.SubIfds, ifdBytes, ifdBytesEntryIndex);
                        break;

                    case TagId.StripOffsets:
                        ReadStripOffsets(destIfd, ifdBytes, ifdBytesEntryIndex);
                        break;

                    case TagId.TileOffsets:
                        ReadTileOffsets(destIfd, ifdBytes, ifdBytesEntryIndex);
                        break;

                    case TagId.StripByteCounts:
                        ReadStripCounts(destIfd, ifdBytes, ifdBytesEntryIndex);
                        break;

                    case TagId.TileByteCounts:
                        ReadTileCounts(destIfd, ifdBytes, ifdBytesEntryIndex);
                        break;

                    case TagId.JpegInterchangeFormat:
                        destIfd.Thumbnail.SourceOffset = NumberConverter.BytesToUInt32(ifdBytes, ifdBytesEntryIndex + 8, byteOrder) + exifFileOffset;
                        destIfd.Thumbnail.SourceStream = sourceReader;
                        break;

                    case TagId.JpegInterchangeFormatLength:
                        destIfd.Thumbnail.Count = (int)NumberConverter.BytesToUInt32(ifdBytes, ifdBytesEntryIndex + 8, byteOrder);
                        break;

                    case TagId.MakerNote:
                    case TagId.OffsetSchema:
                    case TagId.FreeOffsets:
                    case TagId.FreeByteCounts:
                        break;

                    default:
                        var property = CreateProperty(destIfd.IfdId, tagId, tagDetails, ifdBytes, ifdBytesEntryIndex);
                        destIfd.SetPropertyInternal(property);
                        break;
                }
            }

            var nextIfdOffset = NumberConverter.BytesToUInt32(ifdBytes, nextIfdBytesEntryIndex, byteOrder);
            return nextIfdOffset;
        }

        private void ReadStripOffsets(Ifd ifd, byte[] ifdBytes, int ifdBytesEntyIndex)
        {
            ReadTagData(ifdBytes, ifdBytesEntyIndex, out var dataType, out var valueCount, out var tagData);
            ifd.Strip.SourceOffsets = ReadUInts(dataType, valueCount, tagData);
            ifd.Strip.SourceStream = sourceReader;
        }

        private void ReadTileOffsets(Ifd ifd, byte[] ifdBytes, int ifdBytesEntyIndex)
        {
            ReadTagData(ifdBytes, ifdBytesEntyIndex, out var dataType, out var valueCount, out var tagData);
            ifd.Tile.SourceOffsets = ReadUInts(dataType, valueCount, tagData);
            ifd.Tile.SourceStream = sourceReader;
        }

        private void ReadStripCounts(Ifd ifd, byte[] ifdBytes, int ifdBytesEntyIndex)
        {
            ReadTagData(ifdBytes, ifdBytesEntyIndex, out var dataType, out var valueCount, out var tagData);
            ifd.Strip.Counts = ReadUInts(dataType, valueCount, tagData);
        }

        private void ReadTileCounts(Ifd ifd, byte[] ifdBytes, int ifdBytesEntyIndex)
        {
            ReadTagData(ifdBytes, ifdBytesEntyIndex, out var dataType, out var valueCount, out var tagData);
            ifd.Tile.Counts = ReadUInts(dataType, valueCount, tagData);
        }

        private uint[] ReadUInts(DataType dataType, int valueCount, byte[] bytes)
        {
            uint[] uints = new uint[valueCount];

            if (dataType == DataType.UShort)
            {
                for (int i = 0; i < valueCount; i++)
                {
                    uints[i] = NumberConverter.BytesToUInt16(bytes, i * 2, byteOrder);
                }
            }
            else
            {
                for (int i = 0; i < valueCount; i++)
                {
                    uints[i] = NumberConverter.BytesToUInt32(bytes, i * 4, byteOrder);
                }
            }

            return uints;
        }

        private byte[] ReadBytesFromSource(uint fileOffset, int count)
        {
            var data = new byte[count];
            sourceReader.Position = exifFileOffset + fileOffset;

            if (sourceReader.Read(data, 0, count) != count)
            {
                throw new ExifException("Wrong file structure");
            }

            return data;
        }
    }
}