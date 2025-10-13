using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ExifMeta
{
    /// <summary>
    /// One image file directory.
    /// 
    /// In general an ifd is regarded as a bag of properties of different kind. Some properties
    /// are hold a single value or a list of values of the same type. These properties are 
    /// stored in the "properties" list. Other properties are special sub-ifds like exif, gps
    /// or the general sub-ifds property and the very special strip and thumbnail property.
    /// 
    /// All properties have in common all data are stored in the user space. The data are not 
    /// converted, they are just stored as they were passed over from the user. When data are
    /// loaded from a file they are converted into the user space.
    /// </summary>
    [DebuggerDisplay("IfdId = {IfdId}, Properties = {Properties.Count}")]
    public partial class Ifd
    {
        private List<Property> properties;
        private Ifd exifIfd;
        private Ifd gpsIfd;
        private Ifd interopIfd;
        private Ifd globalParamsIfd;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ifdNumber">the ifd number (just for debugging purpose)</param>
        public Ifd() : this(IfdId.Standard) { }

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="ifdNumber">the ifd number (just for debugging purpose)</param>
        /// <param name="ifdId">the ifd ID</param>
        private Ifd(IfdId ifdId)
        {
            IfdId = ifdId;
            properties = new List<Property>();
            Properties = properties;
            Thumbnail = new ThumbnailProperty();
            Strip = new StripProperty();
            Tile = new StripProperty();
        }

        /// <summary>
        /// The ifd ID.
        /// </summary>
        public IfdId IfdId { get; private set; }

        /// <summary>
        /// The properties of the ifd.
        /// </summary>
        public IReadOnlyList<Property> Properties { get; private set; }

        /// <summary>
        /// Thumbnail information.
        /// </summary>
        internal ThumbnailProperty Thumbnail { get; }

        /// <summary>
        /// Strip information
        /// </summary>
        internal StripProperty Strip { get; }

        /// <summary>
        /// Tile information
        /// </summary>
        internal StripProperty Tile { get; }

        /// <summary>
        /// The Exif sub-ifd
        /// </summary>
        public Ifd ExifIfd
        {
            get
            {
                if (exifIfd == null)
                {
                    exifIfd = new Ifd(IfdId.Exif);
                }

                return exifIfd;
            }
        }

        /// <summary>
        /// The GPS sub-ifd
        /// </summary>
        public Ifd GpsIfd
        {
            get
            {
                if (gpsIfd == null)
                {
                    gpsIfd = new Ifd(IfdId.Gps);
                }

                return gpsIfd;
            }
        }

        /// <summary>
        /// The Interop sub-ifd
        /// </summary>
        public Ifd InteropIfd
        {
            get
            {
                if (interopIfd == null)
                {
                    interopIfd = new Ifd(IfdId.Interop);
                }

                return interopIfd;
            }
        }

        /// <summary>
        /// The GlobalParameters ifd
        /// </summary>
        public Ifd GlobalParamsIfd
        {
            get
            {
                if (globalParamsIfd == null)
                {
                    globalParamsIfd = new Ifd(IfdId.GlobalParametersIFD);
                }

                return globalParamsIfd;
            }
        }

        /// <summary>
        /// The sub-ifds
        /// </summary>
        public List<List<Ifd>> SubIfds { get; private set; } = new List<List<Ifd>>();

        /// <summary>
        /// Test whether any meta data are set
        /// </summary>
        /// <returns>true if empty</returns>
        public bool IsEmpty()
        {
            return !(Properties.Count > 0
                     || exifIfd?.Properties.Count > 0
                     || gpsIfd?.Properties.Count > 0
                     || interopIfd?.Properties.Count > 0
                     || globalParamsIfd?.Properties.Count > 0
                     || SubIfds.Count > 0
                     || HasStrips()
                     || HasTiles()
                     || HasThumbnail());
        }

        //////////////////////////////////////////////////////////////////////////
        //// Special property handling

        /// <summary>
        /// Restore the source stream of all data after the original stream was closed.
        /// </summary>
        /// <param name="stream">the new stream</param>
        public void RestoreSource(Stream stream)
        {
            if (Strip.SourceStream != null)
            {
                Strip.SourceStream = stream;
            }

            if (Tile.SourceStream != null)
            {
                Tile.SourceStream = stream;
            }

            if (Thumbnail.SourceStream != null)
            {
                Thumbnail.SourceStream = stream;
            }
        }

        /// <summary>
        /// Test whether the ifd has strip data set.
        /// </summary>
        /// <returns>true if set</returns>
        public bool HasStrips()
        {
            return Strip.SourceOffsets != null;
        }

        /// <summary>
        /// Get the strip data.
        /// </summary>
        /// <param name="stream">the source stream of the data</param>
        /// <param name="sourceOffset">array of offsets within the source stream</param>
        /// <param name="counts">array of counts, one entry for each strip specifying its number of bytes</param>
        public bool GetStrips(out Stream stream, out uint[] sourceOffset, out uint[] counts)
        {
            if (!HasStrips())
            {
                stream = null;
                sourceOffset = null;
                counts = null;

                return false;
            }


            stream = Strip.SourceStream;
            sourceOffset = Strip.SourceOffsets;
            counts = Strip.Counts;

            return true;
        }

        /// <summary>
        /// Set the strip data.
        /// </summary>
        /// <param name="stream">the source stream of the data</param>
        /// <param name="sourceOffset">array of offsets within the source stream</param>
        /// <param name="counts">array of counts, one entry for each strip specifying its number of bytes</param>
        public void SetStrips(Stream stream, uint[] sourceOffset, uint[] counts)
        {
            Strip.SourceStream = stream;
            Strip.SourceOffsets = sourceOffset;
            Strip.Counts = counts;
        }

        /// <summary>
        /// Test whether the ifd has tile data set.
        /// </summary>
        /// <returns>true if set</returns>
        public bool HasTiles()
        {
            return Tile.SourceOffsets != null;
        }

        /// <summary>
        /// Get the tile data.
        /// </summary>
        /// <param name="stream">the source stream of the data</param>
        /// <param name="sourceOffset">array of offsets within the source stream</param>
        /// <param name="counts">array of counts, one entry for each tile specifying its number of bytes</param>
        public bool GetTiles(out Stream stream, out uint[] sourceOffset, out uint[] counts)
        {
            if (!HasStrips())
            {
                stream = null;
                sourceOffset = null;
                counts = null;

                return false;
            }


            stream = Tile.SourceStream;
            sourceOffset = Tile.SourceOffsets;
            counts = Tile.Counts;

            return true;
        }

        /// <summary>
        /// Set the tile data.
        /// </summary>
        /// <param name="stream">the source stream of the data</param>
        /// <param name="sourceOffset">array of offsets within the source stream</param>
        /// <param name="counts">array of counts, one entry for each tile specifying its number of bytes</param>
        public void SetTiles(Stream stream, uint[] sourceOffset, uint[] counts)
        {
            Tile.SourceStream = stream;
            Tile.SourceOffsets = sourceOffset;
            Tile.Counts = counts;
        }

        /// <summary>
        /// Test whether the ifd has thumbnail data set.
        /// </summary>
        /// <returns>true if set</returns>
        public bool HasThumbnail()
        {
            return Thumbnail.SourceOffset != 0;
        }

        /// <summary>
        /// Get the thumbnail data.
        /// </summary>
        /// <param name="sourceStream">the source stream of the data</param>
        /// <param name="sourceOffset">offset of the data within the source stream</param>
        /// <param name="counts">number of bytes</param>
        public void GetThumbnail(out Stream sourceStream, out uint sourceOffset, out int count)
        {
            sourceStream = Thumbnail.SourceStream;
            sourceOffset = Thumbnail.SourceOffset;
            count = Thumbnail.Count;
        }

        /// <summary>
        /// Set the thumbnail data.
        /// </summary>
        /// <param name="sourceStream">the source stream of the data</param>
        /// <param name="sourceOffset">offset of the data within the source stream</param>
        /// <param name="counts">number of bytes</param>
        public void SetThumbnail(Stream sourceStream, uint sourceOffset, int count)
        {
            Thumbnail.SourceStream = sourceStream;
            Thumbnail.SourceOffset = sourceOffset;
            Thumbnail.Count = count;
        }

        //////////////////////////////////////////////////////////////////////////
        //// General property handling

        /// <summary>
        /// Test whether a property exists.
        /// </summary>
        /// <param name="tagId">the tag ID</param>
        /// <returns>true if it exists</returns>
        public bool PropertyExists(TagId tagId)
        {
            var tagDetails = TagDetailsRegistry.GetTagDetails(tagId);
            var ifd = GetIfd(tagDetails.IfdId);
            return ifd.properties.Exists(x => x.TagId == tagId);
        }

        /// <summary>
        /// Test and get a property.
        /// </summary>
        /// <param name="tagId">the tag ID</param>
        /// <returns>a property object</returns>
        /// <returns>true if the tag ID exists otherwise false</returns>
        public bool TryGetProperty(TagId tagId, out Property property)
        {
            var tagDetails = TagDetailsRegistry.GetTagDetails(tagId);
            var ifd = GetIfd(tagDetails.IfdId);
            property = ifd.properties.Find(x => x.TagId == tagId);

            if (property == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get a property.
        /// </summary>
        /// <param name="tagId">the tag ID</param>
        /// <returns>a property object</returns>
        /// <exception cref="ExifException">in case of any error</exception>
        public Property GetProperty(TagId tagId)
        {
            var tagDetails = TagDetailsRegistry.GetTagDetails(tagId);
            var ifd = GetIfd(tagDetails.IfdId);
            var property = ifd.properties.Find(x => x.TagId == tagId);

            if (property == null)
            {
                throw new ExifException("Property does not exist");
            }

            return property;
        }

        /// <summary>
        /// Set a property without ifd checks.
        /// </summary>
        /// <param name="property">the property to set</param>
        internal void SetPropertyInternal(Property property)
        {
            properties.Add(property);
        }

        /// <summary>
        /// Set a property.
        /// </summary>
        /// <param name="property">the property to set</param>
        /// <exception cref="ExifException">in case of any error</exception>
        public void SetProperty(Property property)
        {
            var tagDetails = property.TagDetails;

            if (tagDetails.IsReadOnly)
            {
                throw new ExifException("Property is read-only");
            }

            var ifd = GetIfd(tagDetails.IfdId);

            if (tagDetails.IfdId != ifd.IfdId)
            {
                throw new ExifException("IFD mismatch");
            }

            var currProperty = ifd.properties.Find(x => x.TagId == property.TagId);

            if (currProperty != null)
            {
                ifd.properties.Remove(currProperty);
            }

            ifd.properties.Add(property);
        }

        /// <summary>
        /// Remove a property.
        /// </summary>
        /// <param name="tagId">the tag ID</param>
        /// <returns>true if sucessfully removed</returns>
        public bool RemoveProperty(TagId tagId)
        {
            var tagDetails = TagDetailsRegistry.GetTagDetails(tagId);
            var ifd = GetIfd(tagDetails.IfdId);
            var index = ifd.properties.FindIndex(x => x.TagId == tagId);

            if (index < 0)
            {
                return false;
            }

            ifd.properties.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Get a sub ifd based on a given ifd ID.
        /// </summary>
        /// <param name="destIfdId">the sub ifd ID</param>
        /// <returns>an IFD object</returns>
        /// <exception cref="ExifException">in case of any error</exception>
        private Ifd GetIfd(IfdId destIfdId)
        {
            if (IfdId == IfdId.Standard && destIfdId != IfdId.Standard)
            {
                switch (destIfdId)
                {
                    case IfdId.Exif:
                        return ExifIfd;

                    case IfdId.Gps:
                        return GpsIfd;

                    case IfdId.Interop:
                        return ExifIfd.InteropIfd;

                    default:
                        throw new ExifException("IFD error");
                }
            }
            else
            {
                if (destIfdId != IfdId)
                {
                    throw new ExifException("IFD mismatch");
                }

                return this;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        /// The following methods are convenience methods that get and set 
        /// properties of a given type. These methods cannot be auto-generated
        /// because the types need some special handling.

        /// <summary>
        /// Get a string from a string property.
        /// </summary>
        /// <param name="tagId">the tag ID</param>
        /// <returns>the string value of the property</returns>
        public string GetStringProperty(TagId tagId)
        {
            var property = (StringPropertyBase)GetProperty(tagId);
            return property.Value;
        }

        /// <summary>
        /// Set a string property.
        /// The logic automatically selects the correct string sub-type.
        /// </summary>
        /// <param name="tagId">the tag ID</param>
        /// <param name="value">the string</param>
        /// <exception cref="ExifException"></exception>
        public void SetStringProperty(TagId tagId, string value)
        {
            var tagDetails = TagDetailsRegistry.GetTagDetails(tagId);
            var property = (Property)Activator.CreateInstance(tagDetails.Property, new object[] { tagId, value });
            SetProperty(property);
        }

        /// <summary>
        /// Get a uint property.
        /// The getter supports properties with variable type:
        /// ushort or uint but always returns a uint.
        /// </summary>
        /// <param name="tagId">the tag ID</param>
        /// <returns>a single uint</returns>
        public uint GetUIntProperty(TagId tagId)
        {
            return ((ArrayProperty<uint>)GetProperty(tagId)).Values[0];
        }

        /// <summary>
        /// Set a uint property.
        /// The setter automatically selects the correct type:
        /// </summary>
        /// <param name="tagId">the tag ID</param>
        /// <param name="value">a uint value</param>
        public void SetUIntProperty(TagId tagId, uint value)
        {
            var tagDetails = TagDetailsRegistry.GetTagDetails(tagId);
            var property = (Property)Activator.CreateInstance(tagDetails.Property, new object[] { tagId, value });
            SetProperty(property);
        }
    }
}
