using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ExifMeta
{
    [DebuggerDisplay("{TagId} = {Value}")]
    public class MulticodeUnicodeProperty : MulticodeStringProperty
    {
        public MulticodeUnicodeProperty(TagId tagId, string value) : base(tagId, value, true)
        {
            Validate(typeof(MulticodeUnicodeProperty));
        }
    }

    public class MulticodeAsciiProperty : MulticodeStringProperty
    {
        public MulticodeAsciiProperty(TagId tagId, string value) : base(tagId, value, false)
        {
        }
    }

    [DebuggerDisplay("{TagId} = {Value}")]
    public class MulticodeStringProperty : StringPropertyBase
    {
        private const int IdCodeLength = 8;
        private static readonly byte[] IdCodeUtf16 = new byte[] { (byte)'U', (byte)'N', (byte)'I', (byte)'C', (byte)'O', (byte)'D', (byte)'E', 0 };
        private static readonly byte[] IdCodeAscii = new byte[] { (byte)'A', (byte)'S', (byte)'C', (byte)'I', (byte)'I', 0, 0, 0 };
        private static readonly byte[] IdCodeDefault = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

        public bool UseUnicode { get; }

        public MulticodeStringProperty(TagId tagId, string value, bool useUnicode) : base(tagId)
        {
            Value = value;
            UseUnicode = useUnicode;
            Validate();
            CompleteInitialization(DataType.Undefined, GetByteCount(), false);

            if (TagDetails.Property != typeof(MulticodeStringProperty)
                && TagDetails.Property != typeof(MulticodeAsciiProperty)
                && TagDetails.Property != typeof(MulticodeUnicodeProperty))
            {
                throw new ExifException($"The tag type must be registered for multi code string");
            }
        }

        internal static MulticodeStringProperty FromBinary(TagId tagId,
                                                   int count,
                                                   byte[] bytes,
                                                   ByteOrder byteOrder)
        {
            string value;
            bool useUnicode = true;
            var i = bytes.Length - IdCodeLength;
            var j = IdCodeLength;

            if (ArrayHelper.CompareArrays(bytes, 0, IdCodeUtf16))
            {
                Encoding encoding;

                while (i >= 2 && bytes[j + i - 2] == 0 && bytes[j + i - 1] == 0)
                {
                    i -= 2;
                }

                if (byteOrder == ByteOrder.BigEndian)
                {
                    encoding = Encoding.BigEndianUnicode;
                }
                else
                {
                    encoding = Encoding.Unicode;
                }

                value = encoding.GetString(bytes, j, i);
            }
            else if (ArrayHelper.CompareArrays(bytes, 0, IdCodeAscii)
                     || ArrayHelper.CompareArrays(bytes, 0, IdCodeDefault))
            {
                while (i >= 1 && bytes[j + i - 1] == 0)
                {
                    i--;
                }

                value = Encoding.ASCII.GetString(bytes, j, i);
                useUnicode = false;
            }
            else
            {
                throw new ExifException("unsupported coding");
            }

            return new MulticodeStringProperty(tagId, value, useUnicode);
        }

        internal override int GetByteCount()
        {
            int len;

            if (UseUnicode)
            {
                len = Encoding.Unicode.GetByteCount(Value) + 2;
            }
            else
            {
                len = Encoding.ASCII.GetByteCount(Value) + 1;
            }

            return IdCodeLength + len;
        }

        internal override void RenderData(RenderContext context)
        {
            if (UseUnicode)
            {
                var bytes = Encoding.Unicode.GetBytes(Value);

                context.DestStream.Write(IdCodeUtf16, 0, IdCodeLength);
                context.DestStream.Write(bytes, 0, bytes.Length);
                context.DestStream.WriteByte(0);
                context.DestStream.WriteByte(0);
            }
            else
            {
                var bytes = Encoding.ASCII.GetBytes(Value);

                context.DestStream.Write(IdCodeUtf16, 0, IdCodeLength);
                context.DestStream.Write(bytes, 0, bytes.Length);
                context.DestStream.WriteByte(0);
            }
        }
    }
}
