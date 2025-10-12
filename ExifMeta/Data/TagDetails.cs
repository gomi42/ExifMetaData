using System;
using System.IO;
using System.Linq;

namespace ExifMeta
{
    internal class TagDetails
    {
        public TagDetails(IfdId ifdId,
                          Tag tag,
                          DataType dataType,
                          bool isUser,
                          bool isReadOnly,
                          Type property)
            : this(ifdId,
                   tag,
                   new[] { dataType },
                   isUser,
                   isReadOnly,
                   property)
        {
        }

        public TagDetails(IfdId ifdId,
                          Tag tag,
                          DataType[] dataTypes,
                          bool isUser,
                          bool isReadOnly,
                          Type property)
        {
            IfdId = ifdId;
            Tag = tag;
            DataTypes = dataTypes;
            IsUser = isUser;
            IsReadOnly = isReadOnly;
            Property = property;
            Count = 1;

            if (dataTypes.Contains(DataType.Ascii))
            {
                Count = -1;
            }
        }

        public TagDetails(IfdId ifdId,
                          Tag tag,
                          DataType dataType,
                          bool isUser,
                          bool isReadOnly,
                          Type property,
                          int count)
            : this(ifdId,
                   tag,
                   new[] { dataType },
                   isUser,
                   isReadOnly,
                   property,
                   count)
        {
        }

        public TagDetails(IfdId ifdId,
                          Tag tag,
                          DataType[] dataTypes,
                          bool isUser,
                          bool isReadOnly,
                          Type property,
                          int count)
        {
            IfdId = ifdId;
            Tag = tag;
            DataTypes = dataTypes;
            IsUser = isUser;
            IsReadOnly = isReadOnly;
            Property = property;
            Count = count;
        }

        /// <summary>
        /// The idf ID the is assigned to
        /// </summary>
        public IfdId IfdId { get; private set; }

        /// <summary>
        /// The tag value (from the exif definitions)
        /// </summary>
        public Tag Tag{ get; private set; }

        /// <summary>
        /// The exif data types the tag can used with
        /// </summary>
        public DataType[] DataTypes { get; private set; }

        /// <summary>
        /// True if it is not an administration tag but
        /// a simple user information tag
        /// </summary>
        public bool IsUser { get; private set; }

        /// <summary>
        /// True if the tag cannot be set
        /// </summary>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// The number of required values of type "DataType"
        /// -1 indicate any number.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// The type that manages this tag.
        /// </summary>
        public Type Property { get; private set; }

        /// <summary>
        /// True if the payload is not to be read from file.
        /// </summary>
        public bool DontLoadPayload { get; set; }

        /// <summary>
        /// The display converter.
        /// </summary>
        public IDisplayConverter DisplayConverter { get; set; }
    }
}
