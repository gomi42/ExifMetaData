using System.Diagnostics;

namespace ExifMeta
{
    [DebuggerDisplay("{Tag} = {value}")]
    class BinaryULongTag : BinaryTag
    {
        private uint value;

        public BinaryULongTag(Tag tag, uint value)
        {
            Tag = tag;
            this.value = value;
            DataType = DataType.ULong;
            ValueCount = 1;
            ByteCount = 4;
        }

        public override void RenderData(RenderContext context)
        {
            var bytes = new byte[4];
            NumberConverter.UInt32ToBytes(value, bytes, 0, context.ByteOrder);
            context.DestStream.Write(bytes, 0, 4);
        }
    }
}
