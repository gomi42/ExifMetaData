using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ExifMeta
{
    [DebuggerDisplay("{TagId} = {Value}")]
    public class StringXpProperty : StringPropertyBase
    {
        public StringXpProperty(TagId tagId, string value) : base(tagId)
        {
            Value = value;
            CompleteInitialization(DataType.Byte, GetByteCount());
        }

        internal static StringXpProperty FromBinary(TagId tagId,
                                               int count,
                                               byte[] bytes,
                                               ByteOrder byteOrder)
        {
            return new StringXpProperty(tagId, Encoding.Unicode.GetString(bytes, 0, count - 2));
        }

        internal override int GetByteCount()
        {
            return Encoding.Unicode.GetByteCount(Value) + 2;
        }

        internal override void RenderData(RenderContext context)
        {
            var bytes = Encoding.Unicode.GetBytes(Value);
            context.DestStream.Write(bytes, 0, bytes.Length);
            context.DestStream.WriteByte(0);
            context.DestStream.WriteByte(0);
        }
    }
}
