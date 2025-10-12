using System.Diagnostics;
using System.Linq;

namespace ExifMeta
{
    [DebuggerDisplay("{TagId} = {Values[0]}")]
    public class UShortProperty : ArrayProperty<ushort>
    {
        public UShortProperty(TagId tagId, ushort[] values) : base(tagId, DataType.UShort, values)
        {
        }

        public UShortProperty(TagId tagId, ushort value) : base(tagId, DataType.UShort, value)
        {
        }

        internal static UShortProperty FromBinary(TagId tagId,
                                               int count,
                                               byte[] bytes,
                                               ByteOrder byteOrder)
        {
            var values = new ushort[count];

            for (int i = 0; i < count; i++)
            {
                values[i] = NumberConverter.BytesToUInt16(bytes, i * 2, byteOrder);
            }

            return new UShortProperty(tagId, values);
        }

        internal override int GetByteCount()
        {
            return Values.Length * 2;
        }

        internal override void RenderData(RenderContext context)
        {
            var bytes = new byte[2];

            for (int i = 0; i < Values.Length; i++)
            {
                NumberConverter.UInt16ToBytes(Values[i], bytes, 0, context.ByteOrder);
                context.DestStream.Write(bytes, 0, 2);
            }
        }
    }

    [DebuggerDisplay("{TagId} = {Values[0]}")]
    public class SShortProperty : ArrayProperty<short>
    {
        public SShortProperty(TagId tagId, short[] values) : base(tagId, DataType.UShort, values)
        {
        }

        public SShortProperty(TagId tagId, short value) : base(tagId, DataType.UShort, value)
        {
        }

        internal static SShortProperty FromBinary(TagId tagId,
                                               int count,
                                               byte[] bytes,
                                               ByteOrder byteOrder)
        {
            var values = new short[count];

            for (int i = 0; i < count; i++)
            {
                values[i] = (short)NumberConverter.BytesToUInt16(bytes, i * 2, byteOrder);
            }

            return new SShortProperty(tagId, values);
        }

        internal override int GetByteCount()
        {
            return Values.Length * 2;
        }

        internal override void RenderData(RenderContext context)
        {
            var bytes = new byte[2];

            for (int i = 0; i < Values.Length; i++)
            {
                NumberConverter.UInt16ToBytes((ushort)Values[i], bytes, 0, context.ByteOrder);
                context.DestStream.Write(bytes, 0, 2);
            }
        }
    }
}
