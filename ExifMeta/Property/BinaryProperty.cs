using System.Diagnostics;

namespace ExifMeta
{
    [DebuggerDisplay("{TagId}, Count = {Values.Length}")]
    public class BinaryProperty : ArrayProperty<byte>
    {
        public BinaryProperty(TagId tagId, DataType dataType, byte[] values) : base(tagId, dataType, values)
        {
            Validate(typeof(BinaryProperty));
        }

        internal static BinaryProperty FromBinary(TagId tagId,
                                                  DataType dataType,
                                                  int count,
                                                  byte[] bytes,
                                                  ByteOrder byteOrder)
        {
            return new BinaryProperty(tagId, dataType, bytes);
        }

        internal override int GetByteCount()
        {
            return Values.Length;
        }

        internal override void RenderData(RenderContext context)
        {
            context.DestStream.Write(Values, 0, Values.Length);
        }

        public override string ToString()
        {
            return $"{Values.Length} bytes binary data";
        }
    }
}
