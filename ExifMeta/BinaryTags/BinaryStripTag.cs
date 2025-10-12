namespace ExifMeta
{
    class BinaryStripTag : BinaryTableTag
    {
        private StripProperty stripData;

        public BinaryStripTag(Tag tag, StripProperty stripData)
                : base(tag, stripData.SourceOffsets.Length)
        {
            this.stripData = stripData;
            DataWriteOrder = DataWriteOrder.Strip;
        }

        public override void DryRunAdditionalData(DryRunContext context)
        {
            var countTable = stripData.Counts;
            uint count = (uint)countTable.Length;
            var offsets = new uint[count];

            for (int i = 0; i < count; i++)
            {
                offsets[i] = context.WriteIndex;
                context.WriteIndex += countTable[i];

                if ((context.WriteIndex % 2) != 0)
                {
                    context.WriteIndex++;
                }
            }

            Values = offsets;
        }

        public override void RenderAdditionalData(RenderContext context)
        {
            var sourceOffsets = stripData.SourceOffsets;
            var segments = sourceOffsets.Length;

            var countTable = stripData.Counts;
            var tempBuffer = new byte[60000];

            for (int i = 0; i < segments; i++)
            {
                var remainingByteCount = (int)countTable[i];
                stripData.SourceStream.Position = sourceOffsets[i];
                int bytesToRead = tempBuffer.Length;

                do
                {
                    if (bytesToRead > remainingByteCount)
                    {
                        bytesToRead = remainingByteCount;
                    }

                    if (stripData.SourceStream.Read(tempBuffer, 0, bytesToRead) != bytesToRead)
                    {
                        throw new ExifException("");
                    }

                    context.DestStream.Write(tempBuffer, 0, bytesToRead);
                    remainingByteCount -= bytesToRead;
                }
                while (remainingByteCount > 0);

                if ((countTable[i] % 2) != 0)
                {
                    context.DestStream.WriteByte(0);
                }
            }
        }
    }
}
