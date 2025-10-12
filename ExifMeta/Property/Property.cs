using System;
using System.Linq;

namespace ExifMeta
{
    /// <summary>
    /// Base class of all properties.
    /// </summary>
    public abstract class Property
    {
        private TagDetails tagDetails;

        protected Property(TagId tagId)
        {
            TagId = tagId;
            tagDetails = TagDetailsRegistry.GetTagDetails(tagId);
        }

        protected Property(TagId tagId, DataType dataType, int count)
        {
            TagId = tagId;
            tagDetails = TagDetailsRegistry.GetTagDetails(tagId);
            DataType = dataType;
            Count = count;
            Validate(GetType());
            IsInitialized = true;
        }

        public TagId TagId { get; private set; }

        internal TagDetails TagDetails => tagDetails;

        internal Tag Tag => tagDetails.Tag;

        public DataType DataType { get; private set; }

        public bool IsUser => tagDetails.IsUser;

        public bool IsReadOnly => tagDetails.IsReadOnly;

        public int Count { get; private set; }

        internal IDisplayConverter DisplayConverter => tagDetails.DisplayConverter;

        protected bool IsInitialized { get; private set; }

        internal abstract int GetByteCount();

        internal abstract void RenderData(RenderContext context);

        public override string ToString()
        {
            return $"{Count} {DataType}";
        }

        protected void CompleteInitialization(DataType dataType, int count, bool validate = true)
        {
            if (IsInitialized)
            {
                throw new ExifException("Property is already initialized.");
            }

            IsInitialized = true;
            DataType = dataType;
            Count = count;

            if (validate)
            {
                Validate(GetType());
            }
        }

        protected void ValidateType(Type type)
        {
            var typeToTest = type;

            while (typeToTest != typeof(Property) && typeToTest != typeof(object))
            {
                if (tagDetails.Property == typeToTest)
                {
                        return;
                }

                typeToTest = typeToTest.BaseType;
            }

            throw new ExifException($"The tag is registered for type '{tagDetails.Property.Name}' but used with type '{type.Name}'");
        }

        protected void Validate()
        {
            if (!tagDetails.DataTypes.Contains(DataType))
            {
                throw new ExifException("type missmatch");
            }

            if (tagDetails.Count != -1 && Count != -1 && tagDetails.Count != Count)
            {
                throw new ExifException("count missmatch");
            }
        }
        protected void Validate(Type type)
        {
            ValidateType(type);
            Validate();
        }
    }

    /// <summary>
    /// Base class of single value properties.
    /// Hint: A single value property from the user's perspectiv might consist of
    /// multiple base Exif elements. E.g. DateTime or string.
    /// </summary>
    public abstract class Property<T> : Property
    {
        protected Property(TagId tagId) : base(tagId)
        {
        }

        public Property(TagId tagId, DataType dataType, T value, int count) : base(tagId, dataType, count)
        {
            Value = value;
        }

        public T Value { get; protected set; }

        public override string ToString()
        {
            string result;

            if (DisplayConverter != null)
            {
                result = DisplayConverter.ToString(Value);
            }
            else
            {
                result = Value.ToString();
            }

            return result;
        }
    }

    /// <summary>
    /// The base class of standard array properties.
    /// </summary>
    public abstract class ArrayProperty<T> : Property
    {
        protected ArrayProperty(TagId tagId) : base(tagId)
        {
        }

        public ArrayProperty(TagId tagId, DataType dataType, T[] values) : base(tagId, dataType, values.Length)
        {
            Values = values;
        }

        public ArrayProperty(TagId tagId, DataType dataType, T value) : base(tagId, dataType, 1)
        {
            Values = new T[] { value };
        }

        protected void CompleteInitialization(DataType dataType, T[] values)
        {
            Values = values;
            CompleteInitialization(dataType, values.Length);
        }

        public T[] Values { get; protected set; }

        public override string ToString()
        {
            if (Values.Length > 4)
            {
                var n = typeof(T).Name;
                return $"{Count} {n}";
            }

            Func<T, string> f;

            if (DisplayConverter != null)
            {
                f = x => DisplayConverter.ToString(x);
            }
            else
            {
                f = x => x.ToString();
            }

            return string.Join(" ", Values.Select(f));
        }
    }
}
