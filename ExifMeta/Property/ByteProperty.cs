using System.Diagnostics;
using System.Linq;

namespace ExifMeta
{
    [DebuggerDisplay("{TagId}, Count = {Values.Length}")]
    public class ByteProperty : ArrayProperty<byte>
    {
        public ByteProperty(TagId tagId, byte[] values) : base(tagId, DataType.Byte, values)
        {
            Validate(typeof(ByteProperty));
        }

        public ByteProperty(TagId tagId, byte value) : base(tagId, DataType.Byte, value)
        {
             Validate(typeof(ByteProperty));
        }

        internal static ByteProperty FromBinary(TagId tagId,
                                               int count,
                                               byte[] bytes,
                                               ByteOrder byteOrder)
        {
            return new ByteProperty(tagId, bytes);
        }

        internal override int GetByteCount()
        {
            return Values.Length;
        }

        internal override void RenderData(RenderContext context)
        {
            context.DestStream.Write(Values, 0, Values.Length);
        }
    }

    [DebuggerDisplay("{TagId}, Count = {Values.Length}")]
    public class SByteProperty : ArrayProperty<sbyte>
    {
        public SByteProperty(TagId tagId, sbyte[] values) : base(tagId, DataType.SByte, values)
        {
            Validate(typeof(SByteProperty));
        }

        public SByteProperty(TagId tagId, sbyte value) : base(tagId, DataType.SByte, value)
        {
            Validate(typeof(SByteProperty));
        }

        internal static SByteProperty FromBinary(TagId tagId,
                                               int count,
                                               byte[] bytes,
                                               ByteOrder byteOrder)
        {
            return new SByteProperty(tagId, bytes.Cast<sbyte>().ToArray());
        }

        internal override int GetByteCount()
        {
            return Values.Length;
        }

        internal override void RenderData(RenderContext context)
        {
            context.DestStream.Write(Values.Cast<byte>().ToArray(), 0, Values.Length);
        }
    }
}
