namespace ExifMeta
{
    /// <summary>
    /// Some TIFF header constants
    /// </summary>
    internal static class TiffHeaderConst
    {
        /// <summary>
        /// The TIFF header length (signature + start offset)
        /// </summary>
        public const int TiffHeaderLength = 8;

        /// <summary>
        /// The little endian signature
        /// </summary>
        public static byte[] TiffHeaderSignatureLittleEndian = new byte[] { 0x49, 0x49, 0x2A, 0x00 };

        /// <summary>
        /// The big endian signature
        /// </summary>
        public static byte[] TiffHeaderSignatureBigEndian = new byte[] { 0x4D, 0x4D, 0x00, 0x2A };
    }
}
