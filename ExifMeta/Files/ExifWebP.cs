using System;
using System.IO;
using System.Text;
using ExifMeta;

namespace ExifMetaFile
{
    internal class ExifWebP
    {
        private const string RiffHeaderId = "RIFF";
        private const string WebPFormatId = "WEBP";
        private const string Vp8xChunkId = "VP8X";
        private const string ExifChunkId = "EXIF";
        private const string XmpChunkId = "XMP ";
        private const string IccChunkId = "ICCP";

        public static bool IsWebPFile(Stream stream)
        {
            byte[] tempBuffer = new byte[4];

            stream.Position = 0;
            stream.Read(tempBuffer, 0, 4);

            string headerId = Encoding.ASCII.GetString(tempBuffer, 0, 4);

            if (headerId != RiffHeaderId)
            {
                return false;
            }

            stream.Read(tempBuffer, 0, 4);
            stream.Read(tempBuffer, 0, 4);
            string formatId = Encoding.ASCII.GetString(tempBuffer, 0, 4);
            
            if (formatId != WebPFormatId)
            {
                return false;
            }

            return true;
        }

        public static ImageMetaData Load(Stream sourceStream)
        {
            var imageMetaData = new ImageMetaData();
            imageMetaData.ImageType = ImageType.WebP;

            sourceStream.Position = 0;
            var reader = new BinaryReader(sourceStream, Encoding.ASCII, true);

            string riffId = Encoding.ASCII.GetString(reader.ReadBytes(4));

            if (riffId != RiffHeaderId)
            {
                return null;
            }

            int unusedFileSize = reader.ReadInt32();
            string format = Encoding.ASCII.GetString(reader.ReadBytes(4));

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                string chunkId = Encoding.ASCII.GetString(reader.ReadBytes(4));
                int chunkSize = reader.ReadInt32();
                var position = sourceStream.Position;

                switch (chunkId)
                {
                    case ExifChunkId:
                    {
                        var exifReader = new ExifBinaryReader();
                        imageMetaData.ExifMetaData = exifReader.Read(sourceStream);
                        break;
                    }

                    case XmpChunkId:
                    {
                        imageMetaData.Xmp = reader.ReadBytes(chunkSize);
                        break;
                    }

                    case IccChunkId:
                    {
                        imageMetaData.Icc = reader.ReadBytes(chunkSize);
                        break;
                    }
                }

                reader.BaseStream.Seek(position + chunkSize + (chunkSize % 2), SeekOrigin.Begin);
            }

            reader.Close();

            return imageMetaData;
        }

        public static void Save(Stream sourceStream, Stream destStream, ImageMetaDataWrite metadata)
        {
            sourceStream.Position = 0;
            var reader = new BinaryReader(sourceStream, Encoding.ASCII, true);
            var writer = new BinaryWriter(destStream, Encoding.ASCII, true);

            string riffId = Encoding.ASCII.GetString(reader.ReadBytes(4));

            if (riffId != RiffHeaderId)
            {
                reader.Close();
                writer.Close();

                return;
            }

            int unusedFileSize = reader.ReadInt32();
            string format = Encoding.ASCII.GetString(reader.ReadBytes(4));

            writer.Write(Encoding.ASCII.GetBytes(RiffHeaderId));
            writer.Write((int)0);
            writer.Write(Encoding.ASCII.GetBytes(format));

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                string chunkId = Encoding.ASCII.GetString(reader.ReadBytes(4));
                int chunkSize = reader.ReadInt32();
                var position = sourceStream.Position;
                var copyChunkFromSourceStream = true;

                switch (chunkId)
                {
                    case Vp8xChunkId:
                    {
                        // https://developers.google.com/speed/webp/docs/riff_container?hl=de
                        var bytes = reader.ReadBytes(chunkSize);
                        byte flags = bytes[0];

                        if (metadata.ExifMetaDataOption == ImageMetaDataWriteOption.Remove)
                        {
                            flags &= 0b11110111;
                        }
                        else
                        {
                            flags |= 0b00001000;
                        }

                        if (metadata.XmpOption == ImageMetaDataWriteOption.Remove)
                        {
                            flags &= 0b11111011;
                        }
                        else
                        {
                            flags |= 0b00000100;
                        }

                        if (metadata.IccOption == ImageMetaDataWriteOption.Remove)
                        {
                            flags &= 0b11011111;
                        }
                        else
                        {
                            flags |= 0b00100000;
                        }

                        bytes[0] = flags;
                        writer.Write(Encoding.ASCII.GetBytes(chunkId));
                        writer.Write(chunkSize);
                        writer.Write(bytes);

                        copyChunkFromSourceStream = false;
                        break;
                    }

                    case ExifChunkId:
                    {
                        copyChunkFromSourceStream = metadata.ExifMetaDataOption == ImageMetaDataWriteOption.KeepOriginal;
                        break;
                    }

                    case XmpChunkId:
                    {
                        copyChunkFromSourceStream = metadata.XmpOption == ImageMetaDataWriteOption.KeepOriginal;
                        break;
                    }

                    case IccChunkId:
                    {
                        copyChunkFromSourceStream = metadata.IccOption == ImageMetaDataWriteOption.KeepOriginal;
                        break;
                    }
                }

                if (copyChunkFromSourceStream)
                {
                    writer.Write(Encoding.ASCII.GetBytes(chunkId));
                    writer.Write(chunkSize);

                    var remainingByteCount = chunkSize;
                    var tempBuffer = new byte[60000];
                    int bytesToRead = tempBuffer.Length;

                    do
                    {
                        if (bytesToRead > remainingByteCount)
                        {
                            bytesToRead = remainingByteCount;
                        }

                        if (sourceStream.Read(tempBuffer, 0, bytesToRead) != bytesToRead)
                        {
                            throw new ExifException("");
                        }

                        destStream.Write(tempBuffer, 0, bytesToRead);
                        remainingByteCount -= bytesToRead;
                    }
                    while (remainingByteCount > 0);

                    if ((chunkSize % 2) != 0)
                    {
                        destStream.WriteByte(0);
                    }
                }

                reader.BaseStream.Seek(position + chunkSize + (chunkSize % 2), SeekOrigin.Begin);
            }

            if (metadata.ExifMetaDataOption == ImageMetaDataWriteOption.Overwrite)
            {
                writer.Write(Encoding.ASCII.GetBytes(ExifChunkId));

                var exifWriter = new ExifBinaryWriter(metadata.ExifMetaData);
                var exifBinaryLen = exifWriter.GetSize();
                writer.Write((int)exifBinaryLen);

                exifWriter.Write(destStream);

                if ((exifBinaryLen % 2) != 0)
                {
                    destStream.WriteByte(0);
                }
            }

            if (metadata.XmpOption == ImageMetaDataWriteOption.Overwrite)
            {
                writer.Write(Encoding.ASCII.GetBytes(XmpChunkId));
                writer.Write((int)metadata.Xmp.Length);
                writer.Write(metadata.Xmp);

                if ((metadata.Xmp.Length % 2) != 0)
                {
                    destStream.WriteByte(0);
                }
            }

            if (metadata.IccOption == ImageMetaDataWriteOption.Overwrite)
            {
                writer.Write(Encoding.ASCII.GetBytes(IccChunkId));
                writer.Write((int)metadata.Icc.Length);
                writer.Write(metadata.Icc);

                if ((metadata.Icc.Length % 2) != 0)
                {
                    destStream.WriteByte(0);
                }
            }

            var endposition = destStream.Position;
            destStream.Position = 4;
            writer.Write((int)(endposition - 8));

            reader.Close();
            writer.Close();
        }
    }
}
