using System.Diagnostics;
using System.Linq;

namespace ExifMeta
{
    [DebuggerDisplay("{TagId} = {Values[0]}")]
    public class FloatProperty : ArrayProperty<float>
    {
        public FloatProperty(TagId tagId, float[] values) : base(tagId, DataType.Float, values)
        {
        }

        public FloatProperty(TagId tagId, float value) : base(tagId, DataType.Float, value)
        {
        }

        internal static FloatProperty FromBinary(TagId tagId,
                                               int count,
                                               byte[] bytes,
                                               ByteOrder byteOrder)
        {
            var values = new float[count];

            for (int i = 0; i < count; i++)
            {
                values[i] = NumberConverter.BytesToFloat(bytes, i * 4, byteOrder);
            }

            return new FloatProperty(tagId, values);
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
                NumberConverter.FloatToBytes(Values[i], bytes, 0, context.ByteOrder);
                context.DestStream.Write(bytes, 0, 4);
            }
        }
    }
}
