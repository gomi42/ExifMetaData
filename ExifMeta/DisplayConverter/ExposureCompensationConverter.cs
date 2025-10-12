using System;

namespace ExifMeta
{
    internal class ExposureCompensationConverter : IDisplayConverter
    {
        public string ToString(object objValue)
        {
            var value = (SRational)objValue;
            var numerator = value.Numerator;
            var denominator = value.Denominator;

            if (numerator == 0)
            {
                return "0";
            }

            if ((numerator % denominator) == 0)
            {
                return (numerator / denominator).ToString();
            }

            return $"{value.Numerator}/{value.Denominator}";
        }
    }
}
