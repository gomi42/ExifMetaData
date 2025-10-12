// Based on the works of Hans-Peter Kalb

using System;

namespace ExifMeta
{
    public class GeoCoordinate
    {
        public GeoCoordinate(double degree, double minute, double second, char cardinalPoint)
        {
            Degree = degree;
            Minute = minute;
            Second = second;
            CardinalPoint = cardinalPoint;
        }

        public double Degree { get; private set; } // Integer number: 0 ≤ Degree ≤ 90 (for latitudes) or 180 (for longitudes)
        public double Minute { get; private set; } // Integer number: 0 ≤ Minute < 60
        public double Second { get; private set; } // Fraction number: 0 ≤ Second < 60
        public char CardinalPoint { get; private set; } // For latitudes: 'N' or 'S'; for longitudes: 'E' or 'W'

        public static double ToDecimal(GeoCoordinate value)
        {
            double decimalDegree = value.Degree + value.Minute / 60.0 + value.Second / 3600.0;

            if ((value.CardinalPoint == 'S') || (value.CardinalPoint == 'W'))
            {
                decimalDegree = -decimalDegree;
            }

            return decimalDegree;
        }

        public static GeoCoordinate FromDecimal(double value, bool isLatitude)
        {
            char cardinalPoint;
            double absValue;

            if (value >= 0)
            {
                cardinalPoint = isLatitude ? 'N' : 'E';
                absValue = value;
            }
            else
            {
                cardinalPoint = isLatitude ? 'S' : 'W';
                absValue = -value;
            }

            var degree = Math.Truncate(absValue);
            double frac = (absValue - degree) * 60;
            var minute = Math.Truncate(frac);
            var second = (frac - minute) * 60;
            
            return new GeoCoordinate(degree, minute, second, cardinalPoint);
        }
    }
}
