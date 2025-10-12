using System;

namespace ExifMeta
{
    /// <summary>
    /// Unsigned Rational
    /// </summary>
    public class URational
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="numerator">numerator</param>
        /// <param name="denominator">denominator</param>
        public URational(uint numerator, uint denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        /// <summary>
        /// The numerator.
        /// </summary>
        public uint Numerator { get; protected set; }

        /// <summary>
        /// The Denominator.
        /// </summary>
        public uint Denominator { get; protected set; }

        /// <summary>
        /// Test if zero.
        /// </summary>
        public bool IsZero => Numerator == 0;

        /// <summary>
        /// Test if invalid.
        /// </summary>
        public bool IsValid => Denominator != 0;

        /// <summary>
        /// Convert a decimal to a rational.
        /// </summary>
        /// <param name="value">a decimal</param>
        /// <returns>an URational object</returns>
        /// <exception cref="ArgumentException">in case of any error</exception>
        public static URational FromDecimal(double value)
        {
            if (value < 0)
            {
                throw new ArgumentException("Value cannot be negative");
            }

            var (numerator, denominator) = RationalHelper.ConvertToFraction(value);
            return new URational((uint)numerator, (uint)denominator);
        }

        /// <summary>
        /// Convert to a decimal value.
        /// </summary>
        /// <returns></returns>
        public double ToDecimal()
        {
            return (double)Numerator / Denominator;
        }

        public override string ToString()
        {
            return ToDecimal().ToString();
        }
    }

    /// <summary>
    /// Signed Rational
    /// </summary>
    public class SRational
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="numerator">numerator</param>
        /// <param name="denominator">denominator</param>
        public SRational(int numerator, int denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        /// <summary>
        /// The numerator.
        /// </summary>
        public int Numerator { get; private set; }

        /// <summary>
        /// The Denominator.
        /// </summary>
        public int Denominator { get; private set; }

        /// <summary>
        /// Test if zero.
        /// </summary>
        public bool IsZero => Numerator == 0;

        /// <summary>
        /// Test if invalid.
        /// </summary>
        public bool IsValid => Denominator != 0;

        /// <summary>
        /// Test if negative
        /// </summary>
        public bool IsNegative => Numerator < 0 && Denominator > 0 || Numerator > 0 && Denominator < 0;

        /// <summary>
        /// Test if positive
        /// </summary>
        public bool IsPositive => Numerator > 0 && Denominator > 0 || Numerator > 0 && Denominator > 0;

        /// <summary>
        /// Convert a decimal to a rational.
        /// </summary>
        /// <param name="value">a decimal</param>
        /// <returns>an URational object</returns>
        /// <exception cref="ArgumentException">in case of any error</exception>
        public static SRational FromDecimal(double value)
        {
            var (numerator, denominator) = RationalHelper.ConvertToFraction(value);
            return new SRational(numerator, denominator);
        }

        /// <summary>
        /// Convert to a decimal value.
        /// </summary>
        /// <returns></returns>
        public double ToDecimal()
        {
            return (double)Numerator / Denominator;
        }

        public override string ToString()
        {
            return ToDecimal().ToString();
        }
    }

    internal class RationalHelper
    {
        /// <summary>
        /// Convert a decimal to a fraction.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>(numerator, denominator)</returns>
        public static (int, int) ConvertToFraction(double value)
        {
            // Continued Fraction Algorithm (Kettenbruch-Algorithmus)

            double tolerance = 1e-10;
            int maxIterations = 1000;

            int sign = 1;
            int numerator = 1;
            int denominator = 0;
            int prevNumerator = 0;
            int prevDenominator = 1;
            double b = value;

            if (value < 0)
            {
                value = -value;
                sign = -1;
            }

            for (int i = 0; i < maxIterations; i++)
            {
                int a = (int)Math.Floor(b);
                int tempNumerator = a * numerator + prevNumerator;
                int tempDenominator = a * denominator + prevDenominator;

                if (Math.Abs(value - (double)tempNumerator / tempDenominator) < tolerance)
                {
                    return (sign * tempNumerator, tempDenominator);
                }

                prevNumerator = numerator;
                prevDenominator = denominator;
                numerator = tempNumerator;
                denominator = tempDenominator;

                b = 1.0 / (b - a);
            }

            return (sign * numerator, denominator);
        }
    }
}
