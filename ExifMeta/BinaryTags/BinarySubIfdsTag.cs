using System.Collections.Generic;

namespace ExifMeta
{
    class BinarySubIfdsTag : BinaryTableTag
    {
        private List<List<BinaryIfd>> binSubIfds;

        public BinarySubIfdsTag(List<List<BinaryIfd>> binSubIfds) : base(Tag.SubIFDs, binSubIfds.Count)
        {
            this.binSubIfds = binSubIfds;
            DataWriteOrder = DataWriteOrder.SubIfds;
        }

        public override void DryRunAdditionalData(DryRunContext context)
        {
            var count = (uint)binSubIfds.Count;

            if (count == 0)
            {
                return;
            }

            DataOffset = context.WriteIndex;
            var offsets = new uint[count];

            if (count > 1)
            {
                context.WriteIndex += count * 4;
            }

            for (int i = 0; i < count; i++)
            {
                var binSubIfdChain = binSubIfds[i];
                offsets[i] = context.WriteIndex;

                for (int k = 0; k < binSubIfdChain.Count; k++)
                {
                    context.DryRunBinaryIfd(binSubIfdChain[k], context);
                }
            }

            Values = offsets;
        }

        public override void RenderAdditionalData(RenderContext context)
        {
            for (int i = 0; i < binSubIfds.Count; i++)
            {
                var subIfdChain = binSubIfds[i];
                var subIfdCount = subIfdChain.Count;

                for (int k = 0; k < subIfdCount; k++)
                {
                    context.WriteIfd(subIfdChain[k], k == subIfdCount - 1, context);
                }
            }
        }
    }
}
