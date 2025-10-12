using System.Diagnostics;

namespace ExifMeta
{
    [DebuggerDisplay("Tag = {Tag}")]
    class BinaryTableTag : BinaryTag
    {
        public BinaryTableTag(Tag tag, uint[] values)
        {
            Tag = tag;
            DataType = DataType.ULong;
            Values = values;
            ValueCount = values.Length;
            ByteCount = values.Length * 4;
        }

        protected BinaryTableTag(Tag tag, int valueCount)
        {
            Tag = tag;
            DataType = DataType.ULong;
            ValueCount = valueCount;
            ByteCount = valueCount * 4;
        }

        protected uint[] Values { get; set; }

        public override void RenderData(RenderContext context)
        {
            var bytes = new byte[4];

            for (int i = 0; i < Values.Length; i++)
            {
                NumberConverter.UInt32ToBytes(Values[i], bytes, 0, context.ByteOrder);
                context.DestStream.Write(bytes, 0, 4);
            }
        }
    }
}
