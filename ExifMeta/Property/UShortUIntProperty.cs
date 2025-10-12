using System.Diagnostics;
using System.Linq;

namespace ExifMeta
{
    [DebuggerDisplay("{TagId} = {Values[0]}")]
    public class UShortUIntProperty : ArrayProperty<uint>
    {
        private bool isLong;

        public UShortUIntProperty(TagId tagId, uint[] values) : base(tagId)
        {
            CompleteInitialization(values);
        }

        public UShortUIntProperty(TagId tagId, uint value) : base(tagId)
        {
            CompleteInitialization(new[] { value });
        }

        private void CompleteInitialization(uint[] values)
        {
            isLong = values.Any(x => x < ushort.MinValue || x > ushort.MaxValue);
            var dataType = isLong ? DataType.ULong : DataType.UShort;
            CompleteInitialization(dataType, values);
        }

        internal static UShortUIntProperty FromBinary(TagId tagId,
                                               DataType dataType,
                                               int count,
                                               byte[] bytes,
                                               ByteOrder byteOrder)
        {
            var values = new uint[count];

            if (dataType == DataType.UShort)
            {
                for (int i = 0; i < count; i++)
                {
                    values[i] = NumberConverter.BytesToUInt16(bytes, i * 2, byteOrder);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    values[i] = NumberConverter.BytesToUInt32(bytes, i * 4, byteOrder);
                }
            }

            return new UShortUIntProperty(tagId, values);
        }

        internal override int GetByteCount()
        {
            var size = isLong ? 4 : 2;

            return Values.Length * size;
        }

        internal override void RenderData(RenderContext context)
        {
            if (isLong)
            {
                var bytes = new byte[4];

                for (int i = 0; i < Values.Length; i++)
                {
                    NumberConverter.UInt32ToBytes(Values[i], bytes, 0, context.ByteOrder);
                    context.DestStream.Write(bytes, 0, 4);
                }
            }
            else
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
}
