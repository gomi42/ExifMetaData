// Based on the works of Hans-Peter Kalb

using System;

namespace ExifMeta
{
    public static class ExifGpsExtensions
    {
        public static bool GetDateTaken(this Ifd ifd, out DateTime value)
        {
            return GetDateAndTimeWithMillisecHelper(ifd, TagId.DateTimeOriginal, TagId.SubsecTimeOriginal, out value);
        }

        public static void SetDateTaken(Ifd ifd, DateTime value)
        {
            SetDateAndTimeWithMillisecHelper(ifd, value, TagId.DateTimeOriginal, TagId.SubsecTimeOriginal);
        }

        public static void RemoveDateTaken(Ifd ifd)
        {
            ifd.RemoveProperty(TagId.DateTimeOriginal);
            ifd.RemoveProperty(TagId.SubsecTimeOriginal);
        }

        public static bool GetDateDigitized(this Ifd ifd, out DateTime value)
        {
            return GetDateAndTimeWithMillisecHelper(ifd, TagId.DateTimeDigitized, TagId.SubsecTimeDigitized, out value);
        }

        public static void SetDateDigitized(this Ifd ifd, DateTime value)
        {
            SetDateAndTimeWithMillisecHelper(ifd, value, TagId.DateTimeDigitized, TagId.SubsecTimeDigitized);
        }

        public static void RemoveDateDigitized(Ifd ifd)
        {
            ifd.RemoveProperty(TagId.DateTimeDigitized);
            ifd.RemoveProperty(TagId.SubsecTimeDigitized);
        }

        public static bool GetDateChanged(Ifd ifd, out DateTime value)
        {
            return GetDateAndTimeWithMillisecHelper(ifd, TagId.DateTime, TagId.SubsecTime, out value);
        }

        public static void SetDateChanged(Ifd ifd, DateTime value)
        {
            SetDateAndTimeWithMillisecHelper(ifd, value, TagId.DateTime, TagId.SubsecTime);
        }

        public static void RemoveDateChanged(Ifd ifd)
        {
            ifd.RemoveProperty(TagId.DateTime);
            ifd.RemoveProperty(TagId.SubsecTime);
        }

        public static bool GetGpsLongitude(Ifd ifd, out GeoCoordinate value)
        {
            return GetGpsCoordinateHelper(ifd, TagId.GpsLongitude, TagId.GpsLongitudeRef, "WE", out value);
        }

        public static bool SetGpsLongitude(Ifd ifd, GeoCoordinate value)
        {
            return SetGpsCoordinateHelper(ifd, value, TagId.GpsLongitude, TagId.GpsLongitudeRef, "WE");
        }

        public static void RemoveGpsLongitude(Ifd ifd)
        {
            ifd.RemoveProperty(TagId.GpsLongitude);
            ifd.RemoveProperty(TagId.GpsLongitudeRef);
        }

        public static bool GetGpsLatitude(Ifd ifd, out GeoCoordinate value)
        {
            return GetGpsCoordinateHelper(ifd, TagId.GpsLatitude, TagId.GpsLatitudeRef, "NS", out value);
        }

        public static bool SetGpsLatitude(Ifd ifd, GeoCoordinate value)
        {
            return SetGpsCoordinateHelper(ifd, value, TagId.GpsLatitude, TagId.GpsLatitudeRef, "NS");
        }

        public static void RemoveGpsLatitude(Ifd ifd)
        {
            ifd.RemoveProperty(TagId.GpsLatitude);
            ifd.RemoveProperty(TagId.GpsLatitudeRef);
        }

        public static bool GetGpsAltitude(Ifd ifd, out double value)
        {
            bool success = false;

            var altitude = ifd.GetURationalProperty(TagId.GpsAltitude);

            if (altitude.IsValid)
            {
                value = altitude.ToDecimal();

                var belowSeaLevel = ifd.GetByteProperty(TagId.GpsAltitudeRef);

                if (belowSeaLevel == 1)
                {
                    value = -value;
                }

                success = true;
            }
            else
                value = 0;

            return success;
        }

        public static void SetGpsAltitude(Ifd ifd, double value)
        {
            var altitude = URational.FromDecimal(Math.Abs(value));
            byte belowSeaLevel = 0;

            if (value < 0)
            {
                belowSeaLevel = 1;
            }

            ifd.SetURationalProperty(TagId.GpsAltitude, altitude);
            ifd.SetByteProperty(TagId.GpsAltitudeRef, belowSeaLevel);
        }

        public static void RemoveGpsAltitude(Ifd ifd)
        {
            ifd.RemoveProperty(TagId.GpsAltitude);
            ifd.RemoveProperty(TagId.GpsAltitudeRef);
        }

        public static bool GetGpsDateTimeStamp(Ifd ifd, out DateTime value)
        {
            if (ifd.PropertyExists(TagId.GpsDateStamp))
            {
                value = DateTime.MinValue;
                return false;
            }

            value = ifd.GetDateTimeProperty(TagId.GpsDateStamp);

            if (!ifd.PropertyExists(TagId.GpsTimeStamp))
            {
                return true;
            }

            var values = ifd.GetURationalsProperty(TagId.GpsTimeStamp);

            var hour = values[0];
            var min = values[1];
            var sec = values[2];

            if (!hour.IsValid
                || !min.IsValid
                || !sec.IsValid)
            {
                return false;
            }

            value = value.AddHours(hour.ToDecimal());
            value = value.AddMinutes(min.ToDecimal());
            value = value.AddSeconds(sec.ToDecimal());

            return true;
        }

        public static void SetGpsDateTimeStamp(Ifd ifd, DateTime value)
        {
            URational sec;

            ifd.SetDateTimeProperty(TagId.GpsDateStamp, value.Date);

            TimeSpan ts = value.TimeOfDay;
            URational hour = new URational((uint)ts.Hours, 1);
            URational min = new URational((uint)ts.Minutes, 1);
            int ms = ts.Milliseconds;

            if (ms == 0)
            {
                sec = new URational((uint)ts.Seconds, 1);
            }
            else
            {
                sec = new URational((uint)(ts.Seconds * 1000 + ms), 1000);
            }

            ifd.SetURationalsProperty(TagId.GpsTimeStamp, new[] { hour, min, sec });
        }

        public static void RemoveGpsDateTimeStamp(Ifd ifd)
        {
            ifd.RemoveProperty(TagId.GpsDateStamp);
            ifd.RemoveProperty(TagId.GpsTimeStamp);
        }

        private static bool GetDateAndTimeWithMillisecHelper(Ifd ifd, TagId dateTimeTagId, TagId milliSecTag, out DateTime value)
        {
            bool success = false;

            if (!ifd.PropertyExists(dateTimeTagId))
            {
                value = DateTime.MinValue;
                return false;
            }

            value = ifd.GetDateTimeProperty(dateTimeTagId);

            success = true;

            if (ifd.PropertyExists(milliSecTag))
            {
                var subSec = ifd.GetStringProperty(milliSecTag);
                string s = subSec;
                int len = s.Length;

                if (len > 3)
                {
                    s = s.Substring(0, 3);
                }

                if (int.TryParse(s, out int milliSec) && milliSec >= 0)
                {
                    if (len == 1)
                    {
                        milliSec *= 100;
                    }
                    else
                    if (len == 2)
                    {
                        milliSec *= 10;
                    }

                    value = value.AddMilliseconds(milliSec);
                }
            }

            return success;
        }

        private static void SetDateAndTimeWithMillisecHelper(Ifd ifd, DateTime value, TagId dateTimeTagId, TagId milliSecTagId)
        {
            ifd.SetDateTimeProperty(dateTimeTagId, value);

            int milliSec = value.Millisecond;

            if (milliSec != 0 || ifd.PropertyExists(milliSecTagId))
            {
                string s = milliSec.ToString("000"); // Write exactly 3 decimal digits
                ifd.SetStringProperty(milliSecTagId, s);
            }
        }

        private static bool GetGpsCoordinateHelper(Ifd ifd, TagId tagId, TagId refTagId, string cps, out GeoCoordinate value)
        {
            if (!ifd.PropertyExists(tagId) || !ifd.PropertyExists(refTagId))
            {
                value = null;
                return false;
            }

            var values = ifd.GetURationalsProperty(tagId);
            var gpsRef = ifd.GetStringProperty(refTagId);
            var degree = values[0];
            var minute = values[1];
            var second = values[2];

            if (!degree.IsValid
                || !minute.IsValid
                || !second.IsValid
                || gpsRef.Length != 1)
            {
                value = null;
                return false;
            }

            var cardinalPoint = gpsRef[0];

            if (cps.IndexOf(cardinalPoint) < 0)
            {
                value = null;
                return false;
            }

            value = new GeoCoordinate(degree.ToDecimal(), minute.ToDecimal(), second.ToDecimal(), cardinalPoint);
            return true;
        }

        private static bool SetGpsCoordinateHelper(Ifd ifd, GeoCoordinate value, TagId valueTag, TagId refTag, string cps)
        {
            if (cps.IndexOf(value.CardinalPoint) < 0)
            {
                return false;
            }

            var deg = URational.FromDecimal(value.Degree);
            var min = URational.FromDecimal(value.Minute);
            var sec = URational.FromDecimal(value.Second);

            ifd.SetURationalsProperty(valueTag, new[] { deg, min, sec });
            ifd.SetStringProperty(refTag, value.CardinalPoint.ToString());

            return true;
        }
    }
}
