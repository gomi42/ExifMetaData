using System.Diagnostics;

namespace ExifMeta
{
    [DebuggerDisplay("Tag = {Tag}")]
    class BinaryPropertyTag : BinaryTag
    {
        private Property property;

        public BinaryPropertyTag(Property property)
        {
            this.property = property;
            Tag = property.Tag;
            DataType = property.DataType;
            ValueCount = property.Count;
            ByteCount = property.GetByteCount();
        }

        public override void RenderData(RenderContext context)
        {
            property.RenderData(context);
        }
    }
}
