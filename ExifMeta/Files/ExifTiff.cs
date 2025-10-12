using System.IO;
using System.Linq;
using ExifMeta;

namespace ExifMetaFile
{
    public static class ExifTiff
    {
        public static bool IsTiffFile(Stream stream)
        {
            byte[] tempBuffer = new byte[TiffHeaderConst.TiffHeaderSignatureBigEndian.Length];

            stream.Position = 0;
            stream.Read(tempBuffer, 0, TiffHeaderConst.TiffHeaderSignatureBigEndian.Length);

            if (ArrayHelper.CompareArrays(tempBuffer, 0, TiffHeaderConst.TiffHeaderSignatureBigEndian))
            {
                return true;
            }

            if (ArrayHelper.CompareArrays(tempBuffer, 0, TiffHeaderConst.TiffHeaderSignatureLittleEndian))
            {
                return true;
            }

            return false;
        }

        public static ImageMetaData Load(Stream fileStream)
        {
            fileStream.Position = 0;
            var imageMetaData = new ImageMetaData();
            imageMetaData.ImageType = ImageType.Tiff;

            var exifReader = new ExifBinaryReader();
            exifReader.ReadIfdWithTiffHeader(fileStream, false, out ExifMetaData exifMetaData, out _);
            imageMetaData.ExifMetaData = exifMetaData;

            imageMetaData.Xmp = GetRawData(exifMetaData, TagId.XmpMetadata);
            imageMetaData.Iptc = GetRawData(exifMetaData, TagId.IptcMetadata);
            imageMetaData.Icc = GetRawData(exifMetaData, TagId.InterColorProfile);

            return imageMetaData;
        }

        private static byte[] GetRawData(ExifMetaData exif, TagId tagId)
        {
            var ifd = exif.ImageFileDirectories.FirstOrDefault();

            if (ifd == null)
            {
                return null;
            }

            if (!ifd.PropertyExists(tagId))
            {
                return null;
            }

            var prop = ifd.GetProperty(tagId);

            if (prop is ByteProperty byteProperty)
            {
                return byteProperty.Values;
            }

            if (prop is BinaryProperty binProperty)
            {
                return binProperty.Values;
            }

            return null;
        }

        public static void Save(Stream _, Stream destStream, ImageMetaDataWrite metadata)
        {
            var exifWriter = new ExifBinaryWriter(metadata.ExifMetaData);
            exifWriter.WriteAllWithTiffHeader(destStream);
        }
    }
}
