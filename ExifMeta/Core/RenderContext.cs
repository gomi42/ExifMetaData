using System.IO;

namespace ExifMeta
{
    delegate void WriteIfd(BinaryIfd binIfd, bool isLast, RenderContext context);

    internal class RenderContext
    {
        public RenderContext(Stream destStream, ByteOrder byteOrder, WriteIfd writeIfd)
        {
            DestStream = destStream;
            ByteOrder = byteOrder;
            WriteIfd = writeIfd;
        }

        public Stream DestStream { get; }

        public ByteOrder ByteOrder { get; }

        public WriteIfd WriteIfd { get; }
    }

}
