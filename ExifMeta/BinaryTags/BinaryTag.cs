namespace ExifMeta
{
    abstract class BinaryTag
    {
        protected BinaryTag()
        {
            DataWriteOrder = DataWriteOrder.Default;
        }

        public Tag Tag { get; protected set; }
        public DataType DataType { get; protected set; }
        public int ValueCount { get; protected set; }
        public int ByteCount { get; protected set; }
        public uint DataOffset { get; set; }
        public DataWriteOrder DataWriteOrder  { get; protected set; }

        public abstract void RenderData(RenderContext context);

        public virtual void DryRunAdditionalData(DryRunContext context)
        {
        }

        public virtual void RenderAdditionalData(RenderContext context)
        {
        }
    }
}
