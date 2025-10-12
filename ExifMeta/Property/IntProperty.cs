using System.Diagnostics;
using System.Linq;

namespace ExifMeta
{
    [DebuggerDisplay("{TagId} = {Values[0]}")]
    public class UIntProperty : ArrayProperty<uint>
    {
        public UIntProperty(TagId tagId, uint[] values) : base(tagId, DataType.ULong, values)
        {
        }

        public UIntProperty(TagId tagId, uint value) : base(tagId, DataType.ULong, value)
        {
        }

        internal static UIntProperty FromBinary(TagId tagId,
                                               int count,
                                               byte[] bytes,
                                               ByteOrder byteOrder)
        {
            var values = new uint[count];

            for (int i = 0; i < count; i++)
            {
                values[i] = NumberConverter.BytesToUInt32(bytes, i * 4, byteOrder);
            }

            return new UIntProperty(tagId, values);
        }

        internal override int GetByteCount()
        {
            return Values.Length * 4;
        }

        internal override void RenderData(RenderContext context)
        {
            var bytes = new byte[4];

            for (int i = 0; i < Values.Length; i++)
            {
                NumberConverter.UInt32ToBytes(Values[i], bytes, 0, context.ByteOrder);
                context.DestStream.Write(bytes, 0, 4);
            }
        }
    }

    [DebuggerDisplay("{TagId} = {Values[0]}")]
    public class SIntProperty : ArrayProperty<int>
    {
        public SIntProperty(TagId tagId, int[] values) : base(tagId, DataType.ULong, values) { }

        public SIntProperty(TagId tagId, int value) : base(tagId, DataType.ULong, value) { }

        internal static SIntProperty FromBinary(TagId tagId,
                                               int count,
                                               byte[] bytes,
                                               ByteOrder byteOrder)
        {
            var values = new int[count];
            var tagDetails = TagDetailsRegistry.GetTagDetails(tagId);

            if (!tagDetails.DataTypes.Contains(DataType.SLong))
            {
                throw new ExifException("The tag type must be SLong");
            }

            for (int i = 0; i < count; i++)
            {
                values[i] = (int)NumberConverter.BytesToUInt32(bytes, i * 4, byteOrder);
            }

            return new SIntProperty(tagId, values);
        }

        internal override int GetByteCount()
        {
            return Values.Length * 4;
        }

        internal override void RenderData(RenderContext context)
        {
            var bytes = new byte[4];

            for (int i = 0; i < Values.Length; i++)
            {
                NumberConverter.UInt32ToBytes((uint)Values[i], bytes, 0, context.ByteOrder);
                context.DestStream.Write(bytes, 0, 4);
            }
        }
    }
}
