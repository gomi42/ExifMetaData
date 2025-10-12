using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ExifMeta
{
    [DebuggerDisplay("Ifd, Count = {Tags.Count}")]
    class BinaryIfd
    {
        public BinaryIfd()
        {
            Tags = new List<BinaryTag>();
        }

        public List<BinaryTag> Tags { get; private set; }
        
        public IReadOnlyList<BinaryTag> DataSortedTags { get; private set; }

        public uint NextOffset { get; set; }

        public void FinalizeTags()
        {
            Comparison<BinaryTag> sortTag = delegate (BinaryTag x, BinaryTag y)
            {
                return x.Tag.CompareTo(y.Tag);
            };

            Comparison<BinaryTag> sortDataWriteOrder = delegate (BinaryTag x, BinaryTag y)
            {
                var comp = x.DataWriteOrder.CompareTo(y.DataWriteOrder);

                if (comp != 0)
                {
                    return comp;
                }

                return x.Tag.CompareTo(y.Tag);
            };

            Tags.Sort(sortTag);

            var dataSortedTags = new List<BinaryTag>(Tags);
            dataSortedTags.Sort(sortDataWriteOrder);
            DataSortedTags = dataSortedTags;
        }
    }
}
