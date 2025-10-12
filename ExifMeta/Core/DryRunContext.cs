namespace ExifMeta
{

    delegate void DryRunBinaryIfd(BinaryIfd binIfd, DryRunContext context);

    internal class DryRunContext
    {
        public DryRunContext(uint writeIndex, DryRunBinaryIfd dryRunBinaryIfd)
        {
            WriteIndex = writeIndex;
            DryRunBinaryIfd = dryRunBinaryIfd;
        }

        public uint WriteIndex { get; set; }

        public DryRunBinaryIfd DryRunBinaryIfd { get; }
    }
}
