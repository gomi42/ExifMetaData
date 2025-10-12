using System;

namespace ExifMeta
{
    /// <summary>
    /// Converter to convert exposure time apex value to human readable exposure time.
    /// The converter is currently unused. I cannot decide between this converter
    /// and the Apex1Property (which hides the Apex calculation from the user).
    /// I keep this converter for demonstration purpuses how the idea works.
    /// </summary>
    internal class ExposureTimeApexConverter : IDisplayConverter
    {
        public string ToString(object objValue)
        {
            var value = ((SRational)objValue).ToDecimal();

            if (value < 1)
            {
                var d = Math.Round(1 / Math.Pow(2, -value));
                return $"1/{d}";
            }
            else
            {
                return value.ToString("G1");
            }
        }
    }
}
