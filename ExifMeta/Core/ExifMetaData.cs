using System.Collections.Generic;
using System.IO;

namespace ExifMeta
{
    /// <summary>
    /// The root of all meta data.
    /// </summary>
    public partial class ExifMetaData
    {
        internal uint fileOffset;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExifMetaData()
        {
            ImageFileDirectories = new List<Ifd>();
        }

        /// <summary>
        /// The chained image file directories
        /// </summary>
        public List<Ifd> ImageFileDirectories { get; private set; }

        /// <summary>
        /// Test whether any meta data are set
        /// </summary>
        /// <returns>true if empty</returns>
        public bool IsEmpty()
        {
            foreach (var ifd in ImageFileDirectories)
            {
                if (!ifd.IsEmpty())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Restore the source stream of all data after the original stream was closed.
        /// </summary>
        /// <param name="stream">the new stream</param>
        public void RestoreSource(Stream stream)
        {
            foreach (var ifd in ImageFileDirectories)
            {
                RestoreSource(ifd, stream);
            }
        }

        /// <summary>
        /// Restore the source stream of all data recursively.
        /// </summary>
        /// <param name="ifd">the root ifd</param>
        /// <param name="stream">the new stream</param>
        private void RestoreSource(Ifd ifd, Stream stream)
        {
            ifd.RestoreSource(stream);

            foreach (var subIfdChain in ifd.SubIfds)
            {
                foreach (var subIfd in subIfdChain)
                {
                    RestoreSource(subIfd, stream);
                }
            }
        }
    }
}
