namespace ExifMeta
{
    class BinaryIfdPointerTag : BinaryPointerTag
    {
        BinaryIfd binIfd;

        public BinaryIfdPointerTag(Tag tag, BinaryIfd binIfd) : base(tag)
        {
            this.binIfd = binIfd;
            DataWriteOrder = DataWriteOrder.Ifd;
        }

        public override void DryRunAdditionalData(DryRunContext context)
        {
            DataOffset = context.WriteIndex;
            context.DryRunBinaryIfd(binIfd, context);
        }

        public override void RenderAdditionalData(RenderContext context)
        {
            context.WriteIfd(binIfd, true, context);
        }
    }
}
