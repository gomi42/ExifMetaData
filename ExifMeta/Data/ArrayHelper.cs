namespace ExifMeta
{
    internal static class ArrayHelper
    {
        public static bool CompareArrays(byte[] array1, int startIndex1, byte[] array2)
        {
            if (array1.Length >= (startIndex1 + array2.Length))
            {
                bool isEqual = true;
                int i = startIndex1;

                foreach (byte b in array2)
                {
                    if (array1[i] != b)
                    {
                        isEqual = false;
                        break;
                    }

                    i++;
                }

                return isEqual;
            }
            else
            {
                return false;
            }
        }
    }
}
