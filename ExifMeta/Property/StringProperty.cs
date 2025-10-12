using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ExifMeta
{
    public abstract class StringPropertyBase : Property<string>
    {
        protected StringPropertyBase(TagId tagId) : base(tagId)
        {
        }
    }

    [DebuggerDisplay("{TagId} = {Value}")]
    public class StringProperty : StandardStringPropertyBase
    {
        public StringProperty(TagId tagId, string value) : base(tagId, value, Encoding.ASCII)
        {
        }

        internal static StringProperty FromBinary(TagId tagId,
                                                int count,
                                                byte[] bytes,
                                                ByteOrder byteOrder)
        {
            return new StringProperty(tagId, FromBinaryHelper(tagId,
                                                       Encoding.ASCII,
                                                       count,
                                                       bytes,
                                                       byteOrder));
        }
    }

    [DebuggerDisplay("{TagId} = {Value}")]
    public class StringUtf8Property : StandardStringPropertyBase
    {
        public StringUtf8Property(TagId tagId, string value) : base(tagId, value, Encoding.UTF8)
        {
        }

        internal static StringUtf8Property FromBinary(TagId tagId,
                                                   int count,
                                                   byte[] bytes,
                                                   ByteOrder byteOrder)
        {
            return new StringUtf8Property(tagId, FromBinaryHelper(tagId,
                                                       Encoding.UTF8,
                                                       count,
                                                       bytes,
                                                       byteOrder));
        }
    }

    public abstract class StandardStringPropertyBase : StringPropertyBase
    {
        private Encoding encoding;

        public StandardStringPropertyBase(TagId tagId, string value, Encoding encoding) : base(tagId)
        {
            if (encoding.GetByteCount("A") != 1)
            {
                throw new ExifException("Encoding not supported");
            }

            Value = value;
            this.encoding = encoding;
            CompleteInitialization(DataType.Ascii, GetByteCount());
        }

        internal static string FromBinaryHelper(TagId tagId,
                                               Encoding encoding,
                                               int count,
                                               byte[] bytes,
                                               ByteOrder byteOrder)
        {
            var tagDetails = TagDetailsRegistry.GetTagDetails(tagId);

            if (!tagDetails.DataTypes.Contains(DataType.Ascii))
            {
                throw new ExifException("The tag type must be Ascii");
            }

            return encoding.GetString(bytes, 0, count - 1);
        }

        internal override int GetByteCount()
        {
            return encoding.GetByteCount(Value) + 1;
        }

        internal override void RenderData(RenderContext context)
        {
            var bytes = encoding.GetBytes(Value);
            context.DestStream.Write(bytes, 0, bytes.Length);
            context.DestStream.WriteByte(0);
        }
    }
}
