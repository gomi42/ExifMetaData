using System;
using System.Diagnostics;

namespace ExifMeta
{
    [DebuggerDisplay("{TagId} = {Value}")]
    public class Apex1Property : Property<double>
    {
        public Apex1Property(TagId tagId, double value) : base(tagId, DataType.SRational, value, 1)
        {
        }

        internal static Apex1Property FromBinary(TagId tagId,
                                               int count,
                                               byte[] bytes,
                                               ByteOrder byteOrder)
        {
            var numerator = (int)NumberConverter.BytesToUInt32(bytes, 0, byteOrder);
            var denominator = (int)NumberConverter.BytesToUInt32(bytes, 4, byteOrder);

            var value = (double)numerator / (double)denominator;
            value = Math.Pow(2, -value);

            return new Apex1Property(tagId, value);
        }

        internal override int GetByteCount()
        {
            return 8;
        }

        internal override void RenderData(RenderContext context)
        {
            var bytes = new byte[8];
            double value;

            if (Value > 0)
            {
                value = -Math.Log(Value) / Math.Log(2);
            }
            else
            {
                value = -100;
            }

            var fraction = SRational.FromDecimal(value);

            NumberConverter.UInt32ToBytes((uint)fraction.Numerator, bytes, 0, context.ByteOrder);
            NumberConverter.UInt32ToBytes((uint)fraction.Denominator, bytes, 4, context.ByteOrder);
            context.DestStream.Write(bytes, 0, 8);
        }

        public override string ToString()
        {
            if (Value < 1)
            {
                var d = Math.Round(1 / Value);
                return $"1/{d}";
            }
            else
            {
                return Value.ToString("G1");
            }
        }
    }
}
