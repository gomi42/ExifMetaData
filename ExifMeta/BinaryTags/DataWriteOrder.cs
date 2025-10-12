namespace ExifMeta
{
    // Determines the order in which the additional data of a tag is written to the file.
    // Lowest number first, highes number last
    internal enum DataWriteOrder
    {
        Default = 0,
        Ifd = 1,
        Binary = 2,
        Thumbnail = 3,
        SubIfds = 4,
        Strip = 9999
    }
}
