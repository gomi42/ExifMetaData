using System.Collections.Generic;

namespace ExifMeta
{
    internal static class TagHelper
    {
        private static Dictionary<DataType, int> typeByteCount = new Dictionary<DataType, int>()
            {
                { DataType.Undefined, 1 },
                { DataType.Byte, 1 },
                { DataType.Ascii, 1 },
                { DataType.UShort, 2 },
                { DataType.ULong, 4 },
                { DataType.SLong, 4 },
                { DataType.URational, 8 },
                { DataType.SRational, 8 },
                { DataType.SByte, 1 }, // Only for TIFFs
                { DataType.SShort, 2 }, // Only for TIFFs
                { DataType.Float, 4 }, // Only for TIFFs
                { DataType.Double, 8 }, // Only for TIFFs
                { DataType.Ifd, 4 }, // Only for TIFFs
                { DataType.Utf8, 1 }
            };

        // Get the number of bytes that a tag with the specified type and value count has.
        // If the tag type is invalid, the return value is 0.
        public static int GetTagByteCount(DataType dataType, int valueCount)
        {
            if (typeByteCount.TryGetValue(dataType, out int count))
            {
                return count * valueCount;
            }

            return 0;
        }
    }
}
