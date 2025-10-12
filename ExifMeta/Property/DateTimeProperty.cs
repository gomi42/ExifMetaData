using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ExifMeta
{
    [DebuggerDisplay("{TagId} = {Value}")]
    public class DateTimeProperty : Property<DateTime>
    {
        private const int CharCount = 20;

        public DateTimeProperty(TagId tagId, DateTime value) : base(tagId, DataType.Ascii, value, CharCount) {}

        internal static DateTimeProperty FromBinary(TagId tagId,
                                               int count,
                                               byte[] bytes,
                                               ByteOrder byteOrder)
        {
            var tagDetails = TagDetailsRegistry.GetTagDetails(tagId);

            if (bytes.Length != CharCount
                || bytes[4] != (byte)':'
                || bytes[7] != (byte)':'
                || bytes[10] != (byte)' '
                || bytes[13] != (byte)':'
                || bytes[16] != (byte)':'
                || bytes[19] != 0)
            {
                throw new ExifException("Tag is not of type DateTime");
            }

            var year = ConvertTwoDigitsToInt(bytes, 0) * 100 + ConvertTwoDigitsToInt(bytes, 2);
            var month = ConvertTwoDigitsToInt(bytes, 5);
            var day = ConvertTwoDigitsToInt(bytes, 8);

            var hour = ConvertTwoDigitsToInt(bytes, 11);
            var minute = ConvertTwoDigitsToInt(bytes, 14);
            var second = ConvertTwoDigitsToInt(bytes, 17);

            return new DateTimeProperty(tagId, new DateTime(year, month, day, hour, minute, second));
        }

        internal override int GetByteCount()
        {
            return CharCount;
        }

        internal override void RenderData(RenderContext context)
        {
            var s = Value.ToString("yyyy:MM:dd HH:mm:ss\0");
            var bytes = Encoding.ASCII.GetBytes(s);

            context.DestStream.Write(bytes, 0, 20);
        }

        public static int ConvertTwoDigitsToInt(byte[] bytes, int index)
        {
            int d1 = bytes[index];
            int d2 = bytes[index + 1];

            if (!(d1 >= '0' && d1 <= '9' && d2 >= '0' && d2 <= '9'))
            {
                throw new ExifException("2 digit number in ASCII expected.");
            }

            return (d1 - 0x30) * 10 + (d2 - 0x30);
        }

        public static void ConvertTwoDigitNumber(int value, byte[] bytes, int index)
        {
            bytes[index] = (byte)((value / 10) + 0x30);
            bytes[index + 1] = (byte)((value % 10) + 0x30);
        }
    }

    [DebuggerDisplay("{TagId} = {Value}")]
    public class GpsDateProperty : Property<DateTime>
    {
        private const int CharCount = 11;

        public GpsDateProperty(TagId tagId, DateTime value) : base(tagId, DataType.Ascii, value, CharCount) {}

        internal static GpsDateProperty FromBinary(TagId tagId,
                                               DataType dataType,
                                               int count,
                                               byte[] bytes,
                                               ByteOrder byteOrder)
        {
            var tagDetails = TagDetailsRegistry.GetTagDetails(tagId);

            if (!tagDetails.DataTypes.Contains(DataType.Ascii))
            {
                throw new ExifException("The tag type must be ASCII and must be registered for GpsDate");
            }

            if (bytes.Length != CharCount
                || bytes[4] != (byte)':'
                || bytes[7] != (byte)':'
                || bytes[10] != 0)
            {
                throw new ExifException("Tag is not of type GpsDate");
            }

            var year = DateTimeProperty.ConvertTwoDigitsToInt(bytes, 0) * 100 + DateTimeProperty.ConvertTwoDigitsToInt(bytes, 2);
            var month = DateTimeProperty.ConvertTwoDigitsToInt(bytes, 5);
            var day = DateTimeProperty.ConvertTwoDigitsToInt(bytes, 8);

            return new GpsDateProperty(tagId, new DateTime(year, month, day, 0, 0, 0));
        }

        internal override int GetByteCount()
        {
            return CharCount;
        }

        internal override void RenderData(RenderContext context)
        {
            var s = Value.ToString("yyyy:MM:dd\0");
            var bytes = Encoding.ASCII.GetBytes(s);

            context.DestStream.Write(bytes, 0, 11);
        }
    
        public override string ToString()
        {
            return Value.ToString("yyyy:MM:dd");
        }

    }

    [DebuggerDisplay("{TagId} = {Value}")]
    public class TimeOffsetProperty : Property<TimeSpan>
    {
        private const int CharCount = 7;

        public TimeOffsetProperty(TagId tagId, TimeSpan value) : base(tagId, DataType.Ascii, value, CharCount) {}

        internal static TimeOffsetProperty FromBinary(TagId tagId,
                                               DataType dataType,
                                               int count,
                                               byte[] bytes,
                                               ByteOrder byteOrder)
        {
            var tagDetails = TagDetailsRegistry.GetTagDetails(tagId);

            if (!tagDetails.DataTypes.Contains(DataType.Ascii))
            {
                throw new ExifException("The tag type must be ASCII and must be registered for TimeOffset");
            }

            if (bytes.Length != CharCount
                || bytes[0] != (byte)'-' && bytes[0] != (byte)'+'
                || bytes[3] != (byte)':'
                || bytes[6] != 0)
            {
                throw new ExifException("Tag is not of type TimeOffset");
            }

            int sign = bytes[0] == (byte)'-' ? -1 : 1;
            var hour = sign * DateTimeProperty.ConvertTwoDigitsToInt(bytes, 1);
            var minute = sign * DateTimeProperty.ConvertTwoDigitsToInt(bytes, 4);

            return new TimeOffsetProperty(tagId, new TimeSpan(hour, minute, 0));
        }

        internal override int GetByteCount()
        {
            return CharCount;
        }

        internal override void RenderData(RenderContext context)
        {
            var sign = Value < TimeSpan.Zero ? (byte)'-' : (byte)'+';
            context.DestStream.WriteByte(sign);

            var s = Value.ToString(@"hh\:mm");
            var bytes = Encoding.ASCII.GetBytes(s);

            context.DestStream.Write(bytes, 0, 5);
            context.DestStream.WriteByte(0);
        }

        public override string ToString()
        {
            var sign = Value < TimeSpan.Zero ? '-' : '+';
            var str = Value.ToString(@"hh\:mm");
            return $"{sign}{str}";
        }
    }
}
