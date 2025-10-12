using System;
using System.Collections.Generic;
using System.IO;
using ExifMeta;

namespace ExifMetaFile
{
    public static class ExifDataFile
    {
        delegate bool TestTypeDelegate(Stream sourceStream);
        delegate ImageMetaData LoadDelegate(Stream sourceStream);
        delegate void SaveDelegate(Stream sourceStream, Stream destStream, ImageMetaDataWrite metaData);

        private static readonly List<(TestTypeDelegate Test, LoadDelegate Load, SaveDelegate Save)> FileHandlers =
            new List<(TestTypeDelegate Test, LoadDelegate Load, SaveDelegate Save)>()
            {
                { (ExifJpg.IsJpgFile, ExifJpg.Load, ExifJpg.Save) },
                { (ExifTiff.IsTiffFile, ExifTiff.Load, ExifTiff.Save) },
                { (ExifPng.IsPngFile, ExifPng.Load, ExifPng.Save) },
                { (ExifWebP.IsWebPFile, ExifWebP.Load, ExifWebP.Save) }
            };

        public static ImageMetaData Load(string fileName)
        {
            using (FileStream imageFile = File.OpenRead(fileName))
            {
                return Load(imageFile);
            }
        }

        public static ImageMetaData Load(Stream sourceStream)
        {
            ImageMetaData metaData = null;

            if (GetStreamType(sourceStream, out var fileHandler))
            {
                metaData = fileHandler.Load(sourceStream);
            }

            return metaData;
        }

        public static void Save(Stream sourceStream, Stream destStream, ImageMetaDataWrite metaData)
        {
            if (metaData == null)
            {
                throw new ArgumentNullException(nameof(metaData));
            }

            if (GetStreamType(sourceStream, out var fileHandler))
            {
                fileHandler.Save(sourceStream, destStream, metaData);
            }
            else
            {
                throw new ExifException("file type not supported.");
            }
        }

        private static bool GetStreamType(Stream stream, out (TestTypeDelegate Test, LoadDelegate Load, SaveDelegate Save) fileHandler)
        {
            if (!stream.CanSeek)
            {
                throw new ExifException("Stream must be seekable.");
            }

            if (stream.Length > int.MaxValue)
            {
                throw new ExifException("File type not supported");
            }

            foreach (var handler in FileHandlers)
            {
                if (handler.Test(stream))
                {
                    fileHandler = handler;
                    return true;
                }
            }

            fileHandler = (null, null, null);
            return false;
        }
    }
}
