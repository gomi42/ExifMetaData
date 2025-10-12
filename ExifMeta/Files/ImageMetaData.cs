using System.Text;
using ExifMeta;

namespace ExifMetaFile
{
    public enum ImageType
    {
        Unknown,
        Jpeg,
        Tiff,
        Png,
        WebP
    };

    //////////////////////////////////////////////////////////

    public class ImageMetaData
    {
        public ImageType ImageType;
        public ExifMetaData ExifMetaData;
        public byte[] Xmp;
        public byte[] Icc;
        public byte[] Iptc;
        public JpgComment Comment;
    }

    //////////////////////////////////////////////////////////

    public enum ImageMetaDataWriteOption
    {
        KeepOriginal,
        Overwrite,
        Remove
    }

    //////////////////////////////////////////////////////////

    public class ImageMetaDataWrite : ImageMetaData
    {
        public ImageMetaDataWriteOption ExifMetaDataOption;
        public ImageMetaDataWriteOption XmpOption;
        public ImageMetaDataWriteOption IccOption;
        public ImageMetaDataWriteOption IptcOption;
        public ImageMetaDataWriteOption App13Option;
        public ImageMetaDataWriteOption App14Option;
        public ImageMetaDataWriteOption CommentOption;
    }

    //////////////////////////////////////////////////////////

    public class JpgComment
    {
        private string value;

        public void SetRaw(byte[] bytes)
        {
            value = Encoding.Unicode.GetString(bytes);
        }

        public byte[] GetRaw()
        {
            return Encoding.Unicode.GetBytes(value);
        }

        public void SetComment(string comment)
        {
            value = comment;
        }

        public string GetComment()
        {
            return value;
        }
    }
}
