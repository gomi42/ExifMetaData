using System;

namespace ExifMeta
{
    public class EnumProperty<T> : Property<T>
    {
        public EnumProperty(TagId tagId, T value, DataType dataType) : base(tagId, dataType, value, 1) {}

        internal static Property FromBinary(TagId tagId,
                                               int count,
                                               byte[] bytes,
                                               ByteOrder byteOrder)
        {
            var rawValue = NumberConverter.BytesToUInt16(bytes, 0, byteOrder);
            var value = (T)Enum.ToObject(typeof(T), rawValue);

            var tagDetails = TagDetailsRegistry.GetTagDetails(tagId);
            return (Property)Activator.CreateInstance(tagDetails.Property, new object[] { tagId, value });
        }

        internal override int GetByteCount()
        {
            return TagHelper.GetTagByteCount(DataType, 1);
        }

        internal override void RenderData(RenderContext context)
        {
            var byteCount = TagHelper.GetTagByteCount(DataType, 1);
            var bytes = new byte[byteCount];

            if (byteCount == 2)
            {
                NumberConverter.UInt16ToBytes((ushort)(object)Value, bytes, 0, context.ByteOrder);
            }
            else if (byteCount == 4)
            {
                NumberConverter.UInt32ToBytes((ushort)(object)Value, bytes, 0, context.ByteOrder);
            }

            context.DestStream.Write(bytes, 0, byteCount);
        }
    }
}                                     
                                      
                                      
                                      
                                      