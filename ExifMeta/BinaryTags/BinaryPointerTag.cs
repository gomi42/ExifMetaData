using System.Diagnostics;

namespace ExifMeta
{
    [DebuggerDisplay("Tag = {Tag}")]
    class BinaryPointerTag : BinaryTag
    {
        public BinaryPointerTag(Tag tag)
        {
            Tag = tag;
            DataType = DataType.ULong;
            ValueCount = 1;
            ByteCount = 4;
        }

        public override void RenderData(RenderContext context)
        {
            var bytes = new byte[4];
            NumberConverter.UInt32ToBytes(DataOffset, bytes, 0, context.ByteOrder);
            context.DestStream.Write(bytes, 0, 4);
        }
    }
}
