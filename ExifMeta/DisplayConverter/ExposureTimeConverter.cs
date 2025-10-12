using System;

namespace ExifMeta
{
    /// <summary>
    /// Display human readable exposure time values.
    /// </summary>
    internal class ExposureTimeConverter : IDisplayConverter
    {
        public string ToString(object objValue)
        {
            var value = ((URational)objValue).ToDecimal();
            string result;

            if (value < 1)
            {
                var d = Math.Round(1 / value);
                result = $"1/{d}";
            }
            else
            {
                result = value.ToString("G1");
            }

            return result;
        }
    }
}
