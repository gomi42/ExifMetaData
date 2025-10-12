using System.Diagnostics;

namespace ExifMeta
{
    [DebuggerDisplay("Tag = {Tag}")]
    class BinaryBinTag : BinaryTag
    {
        public BinaryBinTag(Tag tag, DataType dataType, byte[] bytes)
        {
            Tag = tag;
            DataType = dataType;
            ValueCount = bytes.Length;
            ByteCount = bytes.Length;
            Data = bytes;
            DataWriteOrder = DataWriteOrder.Binary;
        }

        public byte[] Data { get; private set; }

        public override void RenderData(RenderContext context)
        {
            context.DestStream.Write(Data, 0, Data.Length);
        }
    }
}
