using System;

namespace ExifMeta
{
    internal static class NumberConverter
    {
        public static ushort BytesBigEndianToUInt16(byte[] sourceData, int offset = 0)
        {
            return (ushort)(((uint)sourceData[offset] << 8) | sourceData[offset + 1]);
        }

        public static void UInt16ToBytesBigEndian(ushort value, byte[] destData, int offset = 0)
        {
            destData[offset] = (byte)(value >> 8);
            destData[offset + 1] = (byte)value;
        }

        public static ushort BytesToUInt16(byte[] sourceData, int offset, ByteOrder byteOrder)
        {
            ushort value;

            if (byteOrder == ByteOrder.BigEndian)
            {
                value = (ushort)(((uint)sourceData[offset] << 8) | sourceData[offset + 1]);
            }
            else
            {
                value = (ushort)(((uint)sourceData[offset + 1] << 8) | sourceData[offset]);
            }

            return value;
        }

        public static void UInt16ToBytes(ushort value, byte[] destData, int offset, ByteOrder byteOrder)
        {
            if (byteOrder == ByteOrder.BigEndian)
            {
                destData[offset] = (byte)(value >> 8);
                destData[offset + 1] = (byte)value;
            }
            else
            {
                destData[offset + 1] = (byte)(value >> 8);
                destData[offset] = (byte)value;
            }
        }

        /////////////////////////////////////////////

        public static void UInt32ToBytesBigEndian(uint value, byte[] destData, int offset = 0)
        {
            UInt32ToBytes(value, destData, offset, ByteOrder.BigEndian);
        }

        public static void UInt32ToBytes(uint value, byte[] destDdata, int offset, ByteOrder byteOrder)
        {
            if (byteOrder == ByteOrder.BigEndian)
            {
                destDdata[offset] = (byte)(value >> 24);
                destDdata[offset + 1] = (byte)(value >> 16);
                destDdata[offset + 2] = (byte)(value >> 8);
                destDdata[offset + 3] = (byte)(value);
            }
            else
            {
                destDdata[offset + 3] = (byte)(value >> 24);
                destDdata[offset + 2] = (byte)(value >> 16);
                destDdata[offset + 1] = (byte)(value >> 8);
                destDdata[offset] = (byte)(value);
            }
        }

        public static uint BytesBigEndianToUInt32(byte[] sourceData, int offset = 0)
        {
            return BytesToUInt32(sourceData, offset, ByteOrder.BigEndian);
        }

        public static uint BytesToUInt32(byte[] sourceData, int offset, ByteOrder byteOrder)
        {
            uint value;

            if (byteOrder == ByteOrder.BigEndian)
            {
                value = ((uint)sourceData[offset] << 24) |
                    ((uint)sourceData[offset + 1] << 16) |
                    ((uint)sourceData[offset + 2] << 8) |
                    (sourceData[offset + 3]);
            }
            else
            {
                value = ((uint)sourceData[offset + 3] << 24) |
                   ((uint)sourceData[offset + 2] << 16) |
                   ((uint)sourceData[offset + 1] << 8) |
                   (sourceData[offset]);
            }

            return value;
        }

        public static ulong BytesToUInt64(byte[] sourceData, int offset, ByteOrder byteOrder)
        {
            ulong value;

            if (byteOrder == ByteOrder.BigEndian)
            {
                value =
                    ((ulong)sourceData[offset] << 56) |
                    ((ulong)sourceData[offset + 1] << 48) |
                    ((ulong)sourceData[offset + 2] << 40) |
                    ((ulong)sourceData[offset + 3] << 32) |
                    ((ulong)sourceData[offset + 4] << 24) |
                    ((ulong)sourceData[offset + 5] << 16) |
                    ((ulong)sourceData[offset + 6] << 8) |
                    ((ulong)sourceData[offset + 7]);
            }
            else
            {
                value =
                    ((ulong)sourceData[offset + 7] << 56) |
                    ((ulong)sourceData[offset + 6] << 48) |
                    ((ulong)sourceData[offset + 5] << 40) |
                    ((ulong)sourceData[offset + 7] << 32) |
                    ((ulong)sourceData[offset + 3] << 24) |
                    ((ulong)sourceData[offset + 2] << 16) |
                    ((ulong)sourceData[offset + 1] << 8) |
                    ((ulong)sourceData[offset]);
            }

            return value;
        }

        public static void UInt64ToBytes(ulong value, byte[] destDdata, int offset, ByteOrder byteOrder)
        {
            if (byteOrder == ByteOrder.BigEndian)
            {
                destDdata[offset] = (byte)(value >> 56);
                destDdata[offset + 1] = (byte)(value >> 48);
                destDdata[offset + 2] = (byte)(value >> 40);
                destDdata[offset + 3] = (byte)(value >> 32);
                destDdata[offset + 4] = (byte)(value >> 24);
                destDdata[offset + 5] = (byte)(value >> 16);
                destDdata[offset + 6] = (byte)(value >> 8);
                destDdata[offset + 7] = (byte)(value);
            }
            else
            {
                destDdata[offset + 7] = (byte)(value >> 56);
                destDdata[offset + 6] = (byte)(value >> 48);
                destDdata[offset + 5] = (byte)(value >> 40);
                destDdata[offset + 4] = (byte)(value >> 32);
                destDdata[offset + 3] = (byte)(value >> 24);
                destDdata[offset + 2] = (byte)(value >> 16);
                destDdata[offset + 1] = (byte)(value >> 8);
                destDdata[offset] = (byte)(value);
            }
        }

        public static float BytesToFloat(byte[] sourceData, int offset, ByteOrder byteOrder)
        {
            var uintValue = BytesToUInt32(sourceData, offset, byteOrder);

            unsafe
            {
                float result = *(float*)&uintValue;
                return result;
            }
        }

        public static void FloatToBytes(float value, byte[] destDdata, int offset, ByteOrder byteOrder)
        {
            uint uintValue;

            unsafe
            {
                uintValue = *(uint*)&value;
            }

            UInt32ToBytes(uintValue, destDdata, offset, byteOrder);
        }

        public static double BytesToDouble(byte[] sourceData, int offset, ByteOrder byteOrder)
        {
            var uintValue = BytesToUInt64(sourceData, offset, byteOrder);

            unsafe
            {
                double result = *(double*)&uintValue;
                return result;
            }
        }

        public static void DoubleToBytes(double value, byte[] destDdata, int offset, ByteOrder byteOrder)
        {
            ulong ulongValue;

            unsafe
            {
                ulongValue = *(ulong*)&value;
            }

            UInt64ToBytes(ulongValue, destDdata, offset, byteOrder);
        }
    }
}
