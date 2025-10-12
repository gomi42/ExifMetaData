using System.Diagnostics;
using System.IO;

namespace ExifMeta
{
    [DebuggerDisplay("{TagId}, Count = {Values.Length}")]
    public class BinaryExternProperty : Property
    {
        public BinaryExternProperty(TagId tagId, DataType dataType, Stream sourceStream, long sourcePosition, int count)
                : base(tagId, dataType, count)
        {
            SourceStream = sourceStream;
            SourcePosition = sourcePosition;
        }

        public Stream SourceStream { get; private set; }
        public long SourcePosition {  get; private set; }

        internal static BinaryExternProperty FromStream(TagId tagId,
                                                        DataType dataType,
                                                        Stream sourceStream,
                                                        long sourcePosition,
                                                        int count)
        {
            return new BinaryExternProperty(tagId, dataType, sourceStream, sourcePosition, count);
        }

        internal override int GetByteCount()
        {
            return Count;
        }

        internal override void RenderData(RenderContext context)
        {
            var remainingByteCount = Count;
            SourceStream.Position = SourcePosition;
            var tempBuffer = new byte[60000];
            int bytesToRead = tempBuffer.Length;

            do
            {
                if (bytesToRead > remainingByteCount)
                {
                    bytesToRead = remainingByteCount;
                }

                if (SourceStream.Read(tempBuffer, 0, bytesToRead) != bytesToRead)
                {
                    throw new ExifException("");
                }

                context.DestStream.Write(tempBuffer, 0, bytesToRead);
                remainingByteCount -= bytesToRead;
            }
            while (remainingByteCount > 0);

            if ((Count % 2) != 0)
            {
                context.DestStream.WriteByte(0);
            }
        }

        public override string ToString()
        {
            return $"{Count} bytes binary data";
        }
    }
}
