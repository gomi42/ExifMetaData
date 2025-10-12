namespace ExifMeta
{
    class BinaryThumbnailTag : BinaryPointerTag
    {
        private ThumbnailProperty thumbnailData;

        public BinaryThumbnailTag(ThumbnailProperty thumbnailData) : base(Tag.JpegInterchangeFormat)
        {
            this.thumbnailData = thumbnailData;
            DataWriteOrder = DataWriteOrder.Thumbnail;
        }
    
        public override void DryRunAdditionalData(DryRunContext context)
        {
            DataOffset = context.WriteIndex;
            var count = (uint)thumbnailData.Count;

            if ((count % 2) != 0)
            {
                count++;
            }

            context.WriteIndex += count;
        }

        public override void RenderAdditionalData(RenderContext context)
        {
            var thumbnailnailSourceOffset = thumbnailData.SourceOffset;
            var thumbnailByteCount = thumbnailData.Count;

            var data = new byte[thumbnailByteCount];
            thumbnailData.SourceStream.Position = thumbnailnailSourceOffset;

            thumbnailData.SourceStream.Read(data, 0, thumbnailByteCount);
            context.DestStream.Write(data, 0, thumbnailByteCount);

            if ((thumbnailByteCount % 2) != 0)
            {
                context.DestStream.WriteByte(0);
            }
        }
    }
}
