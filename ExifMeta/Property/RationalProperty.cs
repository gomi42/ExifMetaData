using System.Diagnostics;

namespace ExifMeta
{
    [DebuggerDisplay("{TagId} = {Values[0].ToDecimal()}")]
    public class URationalProperty : ArrayProperty<URational>
    {
        public URationalProperty(TagId tagId, URational[] values) : base(tagId, DataType.URational, values)
        {
        }

        public URationalProperty(TagId tagId, URational value) : this(tagId, value, true)
        {
        }

        internal URationalProperty(TagId tagId, URational value, bool validate) : base(tagId, DataType.URational, value)
        {
            if (validate)
            {
                Validate(typeof(URationalProperty));
            }
        }

        internal static URationalProperty FromBinary(TagId tagId,
                                               int count,
                                               byte[] bytes,
                                               ByteOrder byteOrder)
        {
            var values = GetValuesFromBinary(tagId,
                                               count,
                                               bytes,
                                               byteOrder);
            return new URationalProperty(tagId, values);
        }

        protected static URational[] GetValuesFromBinary(TagId tagId,
                                               int count,
                                               byte[] bytes,
                                               ByteOrder byteOrder)
        {
            var values = new URational[count];

            for (int i = 0; i < count; i++)
            {
                var index = i * 8;
                var numerator = NumberConverter.BytesToUInt32(bytes, index, byteOrder);
                var denominator = NumberConverter.BytesToUInt32(bytes, index + 4, byteOrder);

                var value = new URational(numerator, denominator);
                values[i] = value;
            }

            return values;
        }

        internal override int GetByteCount()
        {
            return Values.Length * 8;
        }

        internal override void RenderData(RenderContext context)
        {
            var bytes = new byte[8];

            for (int i = 0; i < Values.Length; i++)
            {
                NumberConverter.UInt32ToBytes(Values[i].Numerator, bytes, 0, context.ByteOrder);
                NumberConverter.UInt32ToBytes(Values[i].Denominator, bytes, 4, context.ByteOrder);
                context.DestStream.Write(bytes, 0, 8);
            }
        }
    }

    [DebuggerDisplay("{TagId} = {Values[0].ToDecimal()}")]
    public class SRationalProperty : ArrayProperty<SRational>
    {
        public SRationalProperty(TagId tagId, SRational[] values) : base(tagId, DataType.SRational, values)
        {
        }

        public SRationalProperty(TagId tagId, SRational value) : base(tagId, DataType.SRational, value)
        {
        }

        internal static SRationalProperty FromBinary(TagId tagId,
                                               int count,
                                               byte[] bytes,
                                               ByteOrder byteOrder)
        {
            var values = new SRational[count];

            for (int i = 0; i < count; i++)
            {
                var index = i * 8;
                var numerator = (int)NumberConverter.BytesToUInt32(bytes, index, byteOrder);
                var denominator = (int)NumberConverter.BytesToUInt32(bytes, index + 4, byteOrder);

                var value = new SRational(numerator, denominator);
                values[i] = value;
            }

            return new SRationalProperty(tagId, values);
        }

        internal override int GetByteCount()
        {
            return Values.Length * 8;
        }

        internal override void RenderData(RenderContext context)
        {
            var bytes = new byte[8];

            for (int i = 0; i < Values.Length; i++)
            {
                NumberConverter.UInt32ToBytes((uint)Values[i].Numerator, bytes, 0, context.ByteOrder);
                NumberConverter.UInt32ToBytes((uint)Values[i].Denominator, bytes, 4, context.ByteOrder);
                context.DestStream.Write(bytes, 0, 8);
            }
        }
    }
}
