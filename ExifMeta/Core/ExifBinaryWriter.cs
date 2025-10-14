using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;

namespace ExifMeta
{
    public class ExifBinaryWriter
    {
        private ExifMetaData exifMeta;
        private ByteOrder byteOrder = ByteOrder.LittleEndian;

        public ExifBinaryWriter(ExifMetaData exifMeta)
        {
            this.exifMeta = exifMeta;
        }

        public ByteOrder ByteOrder
        {
            get => byteOrder;
            set => byteOrder = value;
        }

        /// <summary>
        /// Serialize to a byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] WriteBinary()
        {
            using (var destStream = new MemoryStream())
            {
                Write(destStream);
                return destStream.ToArray();
            }
        }

        /// <summary>
        /// Serialize into a stream.
        /// </summary>
        /// <param name="destStream"></param>
        public void Write(Stream destStream)
        {
            var renderContext = new RenderContext(destStream, byteOrder, WriteIfd);

            WriteTiffHeader(destStream, TiffHeaderConst.TiffHeaderLength);
            var binIfds = DryRunAll(TiffHeaderConst.TiffHeaderLength);
            WriteIfdChain(binIfds, renderContext);
        }

        /// <summary>
        /// Get the size of the final serialized data.
        /// </summary>
        /// <returns></returns>
        public uint GetSize()
        {
            var dryRunContext = new DryRunContext(TiffHeaderConst.TiffHeaderLength, DryRunBinaryIfd);

            foreach (var ifd in exifMeta.ImageFileDirectories)
            {
                DryRunIfd(ifd, dryRunContext);
            }

            return dryRunContext.WriteIndex;
        }

        private void WriteTiffHeader(Stream destStream, int exifBlockOffset)
        {
            if (byteOrder == ByteOrder.BigEndian)
            {
                destStream.Write(TiffHeaderConst.TiffHeaderSignatureBigEndian, 0, TiffHeaderConst.TiffHeaderSignatureBigEndian.Length);
            }
            else if (byteOrder == ByteOrder.LittleEndian)
            {
                destStream.Write(TiffHeaderConst.TiffHeaderSignatureLittleEndian, 0, TiffHeaderConst.TiffHeaderSignatureLittleEndian.Length);
            }

            var tiffHeader = new byte[4];
            NumberConverter.UInt32ToBytes((uint)exifBlockOffset, tiffHeader, 0, byteOrder);
            destStream.Write(tiffHeader, 0, 4);
        }

        private void WriteIfdChain(List<BinaryIfd> binIfds, RenderContext renderContext)
        {
            var last = binIfds.Count - 1;

            for (int i = 0; i <= last; i++)
            {
                var binIfd = binIfds[i];
                WriteIfd(binIfd, i == last, renderContext);
            }
        }

        private List<BinaryIfd> DryRunAll(uint exifBlockOffset)
        {
            var binIfds = new List<BinaryIfd>();
            var dryRunContext = new DryRunContext(exifBlockOffset, DryRunBinaryIfd);

            foreach (var ifd in exifMeta.ImageFileDirectories)
            {
                var binIfd = DryRunIfd(ifd, dryRunContext);
                binIfds.Add(binIfd);
            }

            return binIfds;
        }

        private BinaryIfd DryRunIfd(Ifd ifd, DryRunContext context)
        {
            var binIfd = CreateBinaryIfd(ifd);
            DryRunBinaryIfd(binIfd, context);

            return binIfd;
        }

        private BinaryIfd CreateBinaryIfd(Ifd ifd)
        {
            Comparison<BinaryTag> sort = delegate (BinaryTag x, BinaryTag y)
                                            {
                                                return x.Tag.CompareTo(y.Tag);
                                            };

            if (ifd.IsEmpty())
            {
                return null;
            }

            var binIfd = new BinaryIfd();

            foreach (var property in ifd.Properties)
            {
                var binTag = new BinaryPropertyTag(property);
                binIfd.Tags.Add(binTag);
            }

            AddIfdPointerTag(ifd.ExifIfd, binIfd, Tag.ExifIfdPointer);
            AddIfdPointerTag(ifd.GpsIfd, binIfd, Tag.GpsIfdPointer);
            AddIfdPointerTag(ifd.InteropIfd, binIfd, Tag.InteropIfdPointer);
            AddIfdPointerTag(ifd.GlobalParamsIfd, binIfd, Tag.GlobalParametersIFD);
            AddExifTags(ifd, binIfd);
            AddGpsTags(ifd, binIfd);
            AddThumbnailTags(ifd, binIfd);
            AddSubIfdsTags(ifd, binIfd);
            AddStripTags(ifd, binIfd);
            AddTileTags(ifd, binIfd);

            binIfd.FinalizeTags();

            return binIfd;
        }

        private void AddIfdPointerTag(Ifd ifd, BinaryIfd binIfd, Tag pointerTag)
        {
            if (ifd.IsEmpty())
            {
                return;
            }

            var binIfd2 = CreateBinaryIfd(ifd);
            var binTag = new BinaryIfdPointerTag(pointerTag, binIfd2);
            binIfd.Tags.Add(binTag);
        }

        private void AddExifTags(Ifd ifd, BinaryIfd binIfd)
        {
            if (ifd.IfdId != IfdId.Exif || ifd.IsEmpty())
            {
                return;
            }

            var existingProperty = ifd.Properties.FirstOrDefault(x => x.TagId == TagId.ExifVersion);

            if (existingProperty == null)
            {
                var binTag = new BinaryBinTag(Tag.ExifVersion, DataType.Undefined, Encoding.ASCII.GetBytes("0231"));
                binIfd.Tags.Add(binTag);
            }
        }

        private void AddGpsTags(Ifd ifd, BinaryIfd binIfd)
        {
            if (ifd.IfdId != IfdId.Gps || ifd.IsEmpty())
            {
                return;
            }

            var existingProperty = ifd.Properties.FirstOrDefault(x => x.TagId == TagId.GpsVersionId);

            if (existingProperty == null)
            {
                var binTag = new BinaryBinTag(Tag.GpsVersionId, DataType.Byte, new byte[] { 2, 4, 0, 0 });
                binIfd.Tags.Add(binTag);
            }
        }

        private void AddThumbnailTags(Ifd ifd, BinaryIfd binIfd)
        {
            if (!ifd.HasThumbnail())
            {
                return;
            }

            var binTag = new BinaryThumbnailTag(ifd.Thumbnail);
            binIfd.Tags.Add(binTag);

            var binTag2 = new BinaryULongTag(Tag.JpegInterchangeFormatLength, (uint)ifd.Thumbnail.Count);
            binIfd.Tags.Add(binTag2);
        }

        private void AddSubIfdsTags(Ifd ifd, BinaryIfd binIfd)
        {
            var count = ifd.SubIfds.Count;

            if (count == 0)
            {
                return;
            }

            var binSubIfds = new List<List<BinaryIfd>>();

            for (int i = 0; i < count; i++)
            {
                var binSubIfdChain = new List<BinaryIfd>();
                binSubIfds.Add(binSubIfdChain);

                var subIfdChain = ifd.SubIfds[i];

                for (int k = 0; k < subIfdChain.Count; k++)
                {
                    var binSubIfd = CreateBinaryIfd(subIfdChain[k]);
                    binSubIfdChain.Add(binSubIfd);
                }
            }

            var binTag = new BinarySubIfdsTag(binSubIfds);
            binIfd.Tags.Add(binTag);
        }

        private void AddStripTags(Ifd ifd, BinaryIfd binIfd)
        {
            if (!ifd.HasStrips())
            {
                return;
            }

            var binTag = new BinaryStripTag(Tag.StripOffsets, ifd.Strip);
            binIfd.Tags.Add(binTag);

            var binTag2 = new BinaryTableTag(Tag.StripByteCounts, ifd.Strip.Counts);
            binIfd.Tags.Add(binTag2);
        }

        private void AddTileTags(Ifd ifd, BinaryIfd binIfd)
        {
            if (!ifd.HasTiles())
            {
                return;
            }

            var binTag = new BinaryStripTag(Tag.TileOffsets, ifd.Tile);
            binIfd.Tags.Add(binTag);

            var binTag2 = new BinaryTableTag(Tag.TileByteCounts, ifd.Strip.Counts);
            binIfd.Tags.Add(binTag2);
        }

        private void DryRunBinaryIfd(BinaryIfd binIfd, DryRunContext context)
        {
            uint tagCount = (uint)binIfd.Tags.Count;
            context.WriteIndex += 2 + tagCount * 12 + 4; // ifd

            foreach (var tag in binIfd.DataSortedTags)
            {
                DryRunTagData(tag, context);
            }

            foreach (var tag in binIfd.DataSortedTags)
            {
                tag.DryRunAdditionalData(context);
            }

            binIfd.NextOffset = context.WriteIndex;
        }

        private void DryRunTagData(BinaryTag binTag, DryRunContext context)
        {
            var byteCount = (uint)binTag.ByteCount;

            if (byteCount > 4)
            {
                if ((byteCount % 2) != 0)
                {
                    byteCount++;
                }

                binTag.DataOffset = context.WriteIndex;
                context.WriteIndex += byteCount;
            }
        }

        private void WriteIfd(BinaryIfd binIfd, bool isLastIfd, RenderContext renderContext)
        {
            int tagCount = binIfd.Tags.Count;

            var tempBuffer2 = new byte[2];
            NumberConverter.UInt16ToBytes((ushort)tagCount, tempBuffer2, 0, renderContext.ByteOrder);
            renderContext.DestStream.Write(tempBuffer2, 0, 2);

            foreach (var tag in binIfd.Tags)
            {
                WriteTagDirectoryEntry(tag, renderContext);
            }

            uint nextOffset = 0;

            if (!isLastIfd)
            {
                nextOffset = binIfd.NextOffset;
            }

            var tempBuffer4 = new byte[4];
            NumberConverter.UInt32ToBytes(nextOffset, tempBuffer4, 0, renderContext.ByteOrder);
            renderContext.DestStream.Write(tempBuffer4, 0, 4);

            foreach (var tag in binIfd.DataSortedTags)
            {
                WriteTagData(tag, renderContext);
            }

            foreach (var tag in binIfd.DataSortedTags)
            {
                tag.RenderAdditionalData(renderContext);
            }
        }

        private void WriteTagDirectoryEntry(BinaryTag tag, RenderContext renderContext)
        {
            var tempBuffer2 = new byte[2];
            var tempBuffer4 = new byte[4];

            NumberConverter.UInt16ToBytes((ushort)tag.Tag, tempBuffer2, 0, renderContext.ByteOrder);
            renderContext.DestStream.Write(tempBuffer2, 0, 2);

            NumberConverter.UInt16ToBytes((ushort)tag.DataType, tempBuffer2, 0, renderContext.ByteOrder);
            renderContext.DestStream.Write(tempBuffer2, 0, 2);

            NumberConverter.UInt32ToBytes((uint)tag.ValueCount, tempBuffer4, 0, renderContext.ByteOrder);
            renderContext.DestStream.Write(tempBuffer4, 0, 4);

            var byteCount = tag.ByteCount;

            if (byteCount <= 4)
            {
                tag.RenderData(renderContext);

                for (int i = byteCount; i < 4; i++)
                {
                    renderContext.DestStream.WriteByte(0);
                }
            }
            else
            {
                NumberConverter.UInt32ToBytes(tag.DataOffset, tempBuffer4, 0, renderContext.ByteOrder);
                renderContext.DestStream.Write(tempBuffer4, 0, 4);
            }
        }

        private void WriteTagData(BinaryTag tag, RenderContext renderContext)
        {
            var byteCount = tag.ByteCount;

            if (byteCount > 4)
            {
                tag.RenderData(renderContext);

                if ((byteCount % 2) != 0)
                {
                    renderContext.DestStream.WriteByte(0);
                }
            }
        }
    }
}
