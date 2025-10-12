using System.Diagnostics;
using System.Linq;

namespace ExifMeta
{
    [DebuggerDisplay("{TagId} = {Values[0]}")]
    public class DoubleProperty : ArrayProperty<double>
    {
        public DoubleProperty(TagId tagId, double[] values) : base(tagId, DataType.Double, values)
        {
        }

        public DoubleProperty(TagId tagId, double value) : base(tagId, DataType.Double, value)
        {
        }

        internal static DoubleProperty FromBinary(TagId tagId,
                                               int count,
                                               byte[] bytes,
                                               ByteOrder byteOrder)
        {
            var values = new double[count];

            for (int i = 0; i < count; i++)
            {
                values[i] = NumberConverter.BytesToDouble(bytes, i * 4, byteOrder);
            }

            return new DoubleProperty(tagId, values);
        }

        internal override int GetByteCount()
        {
            return Values.Length * 4;
        }

        internal override void RenderData(RenderContext context)
        {
            var bytes = new byte[8];

            for (int i = 0; i < Values.Length; i++)
            {
                NumberConverter.DoubleToBytes(Values[i], bytes, 0, context.ByteOrder);
                context.DestStream.Write(bytes, 0, 4);
            }
        }
    }
}
