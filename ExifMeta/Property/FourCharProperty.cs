using System.Diagnostics;
using System.Text;

namespace ExifMeta
{
    [DebuggerDisplay("{TagId} = {Value}")]
    public class FourCharProperty : StringPropertyBase
    {
        public FourCharProperty(TagId tagId, string value) : base(tagId)
        {
            if (value.Length != 4)
            {
                throw new ExifException("string must be exactly 4 char long");
            }

            Value = value;
            CompleteInitialization(TagDetails.DataTypes[0], 4);
        }

        internal static FourCharProperty FromBinary(TagId tagId,
                                               int count,
                                               byte[] bytes,
                                               ByteOrder byteOrder)
        {
            return new FourCharProperty(tagId, Encoding.ASCII.GetString(bytes, 0, 4));
        }

        internal override int GetByteCount()
        {
            return 4;
        }

        internal override void RenderData(RenderContext context)
        {
            var bytes = Encoding.ASCII.GetBytes(Value);
            context.DestStream.Write(bytes, 0, 4);
        }
    }
}
