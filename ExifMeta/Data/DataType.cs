namespace ExifMeta
{
    /// <summary>
    /// The exif data types.
    /// </summary>
    public enum DataType
    {
        Byte = 1,
        Ascii = 2,
        UShort = 3,
        ULong = 4,
        URational = 5,
        SByte = 6, // Only for TIFFs
        Undefined = 7,
        SShort = 8, // Only for TIFFs
        SLong = 9,
        SRational = 10,
        Float = 11, // Only for TIFFs
        Double = 12, // Only for TIFFs
        Ifd = 13,
        Utf8 = 129,
        Unknown = 9999
    }
}
