using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using ExifMeta;
using ExifMetaFile;

namespace Tests
{
    public partial class MainWindow : Window
    {
        private string globalFilename = "<drag & drop a file>";
        public MainWindow()
        {
            InitializeComponent();
            AllowDrop = true;
            Filename.Text = globalFilename;
        }

        private void Button_CopyFile(object sender, RoutedEventArgs e)
        {
            Copy(globalFilename);
        }

        private void Copy(string filename)
        {
            // read the metadata
            FileStream sourceStream = File.OpenRead(filename);
            var metaData = ExifDataFile.Load(sourceStream);

            // modify the metadata
            var srcEx = metaData.ExifMetaData;
            var ifd0 = srcEx.ImageFileDirectories[0];
            ifd0.SetArtist("John Doe");

            // analyze XMP
            var xmpString = Encoding.UTF8.GetString(metaData.Xmp);

            // prepare the metadata for writing: keep, remove, overwrite
            var mdw = new ImageMetaDataWrite();

            mdw.ExifMetaData = metaData.ExifMetaData;
            mdw.ExifMetaDataOption = metaData.ExifMetaData != null ? ImageMetaDataWriteOption.Overwrite : ImageMetaDataWriteOption.KeepOriginal;

            mdw.Xmp = metaData.Xmp;
            mdw.XmpOption = metaData.Xmp != null ? ImageMetaDataWriteOption.Overwrite : ImageMetaDataWriteOption.KeepOriginal;

            mdw.Icc = metaData.Icc;
            mdw.IccOption = metaData.Icc != null ? ImageMetaDataWriteOption.Overwrite : ImageMetaDataWriteOption.KeepOriginal;

            mdw.Iptc = metaData.Iptc;
            mdw.IptcOption = metaData.Iptc != null ? ImageMetaDataWriteOption.Overwrite : ImageMetaDataWriteOption.KeepOriginal;

            mdw.App13Option = ImageMetaDataWriteOption.KeepOriginal;
            mdw.App14Option = ImageMetaDataWriteOption.KeepOriginal;

            //mdw.ExifMetaDataOption = ImageMetaDataWriteOption.Remove;
            //mdw.XmpOption = ImageMetaDataWriteOption.Remove;
            //mdw.IccOption = ImageMetaDataWriteOption.Remove;
            //mdw.IptcOption = ImageMetaDataWriteOption.Remove;

            // write the new file
            var destStream = File.Create(GetCopyFilename(filename));
            ExifDataFile.Save(sourceStream, destStream, mdw);

            sourceStream.Close();
            destStream.Close();
        }

        public string GetCopyFilename(string sourceFilename)
        {
            int backslashPosition = sourceFilename.LastIndexOf('\\');
            int dotPosition = sourceFilename.LastIndexOf('.');

            if (dotPosition <= backslashPosition)
            {
                dotPosition = sourceFilename.Length;
            }

            return sourceFilename.Insert(dotPosition, "_2");
        }

        private void Button_Click_CopyTiff(object sender, RoutedEventArgs e)
        {
            FileStream sourceStream = File.OpenRead(globalFilename);
            var metaData = ExifDataFile.Load(sourceStream);

            if (metaData.ImageType != ImageType.Tiff)
            {
                sourceStream.Close();
                return;
            }

            var srcEx = metaData.ExifMetaData;
            var srcIfd = srcEx.ImageFileDirectories[0];

            var subFileType = srcIfd.GetNewSubfileType();
            var width = srcIfd.GetImageWidth();
            var height = srcIfd.GetImageHeight();
            var samplesPerPixel = srcIfd.GetSamplesPerPixel();
            var bitsPerSample = srcIfd.GetBitsPerSample();
            var photo = srcIfd.GetPhotometricInterpretation();
            var planar = srcIfd.GetPlanarConfiguration();
            var compression = srcIfd.GetCompression();
            var predictor = srcIfd.GetPredictor();

            var dstEx = new ExifMetaData();
            var dstIfd = new Ifd();
            dstEx.ImageFileDirectories.Add(dstIfd);

            dstIfd.SetNewSubfileType(subFileType);
            dstIfd.SetImageWidth(width);
            dstIfd.SetImageHeight(height);
            dstIfd.SetSamplesPerPixel(samplesPerPixel);
            dstIfd.SetBitsPerSample(bitsPerSample);
            dstIfd.SetPhotometricInterpretation(photo);
            dstIfd.SetPlanarConfiguration(planar);
            dstIfd.SetCompression(compression);
            dstIfd.SetPredictor(predictor);

            if (srcIfd.PropertyExists(TagId.InterColorProfile))
            {
                dstIfd.SetInterColorProfile(srcIfd.GetInterColorProfile());
            }

            if (srcIfd.HasStrips())
            {
                dstIfd.SetRowsPerStrip(srcIfd.GetRowsPerStrip());
                srcIfd.GetStrips(out var stream, out var offsets, out var counts);
                dstIfd.SetStrips(stream, offsets, counts);
            }

            if (srcIfd.HasTiles())
            {
                dstIfd.SetTileWidth(srcIfd.GetTileWidth());
                dstIfd.SetTileLength(srcIfd.GetTileLength());
                srcIfd.GetTiles(out var stream, out var offsets, out var counts);
                dstIfd.SetTiles(stream, offsets, counts);
            }

            var destStream = File.Create(GetCopyFilename(globalFilename));
            var exifWriter = new ExifBinaryWriter(dstEx);
            exifWriter.Write(destStream);

            sourceStream.Close();
            destStream.Close();
        }

        private void Button_Click_CopyUser(object sender, RoutedEventArgs e)
        {
            var newExif = CopyUser(globalFilename);
        }

        private ExifMetaData CopyUser(string filename)
        {
            FileStream sourceStream = File.OpenRead(filename);
            var metaData = ExifDataFile.Load(sourceStream);

            var newExif = new ExifMetaData();
            var newIfd = new Ifd();
            newExif.ImageFileDirectories.Add(newIfd);

            var ifd0 = metaData.ExifMetaData.ImageFileDirectories[0];

            CopyUserProps(ifd0, newIfd);
            CopyUserProps(ifd0.ExifIfd, newIfd.ExifIfd);
            CopyUserProps(ifd0.GpsIfd, newIfd.GpsIfd);

            // User data don't hold a reference to the original source stream.
            // We can safely close the stream.
            sourceStream.Close();

            return newExif;
        }

        private static void CopyUserProps(Ifd source, Ifd dest)
        {
            foreach (var prop in source.Properties)
            {
                if (prop.IsUser)
                {
                    dest.SetProperty(prop);
                }
            }
        }

        private void Button_ShowExif(object sender, RoutedEventArgs e)
        {
            ShowExifData(globalFilename);
        }

        private void ShowExifData(string filename)
        {
            FileStream sourceStream = File.OpenRead(filename);
            var metaData = ExifDataFile.Load(sourceStream);
            sourceStream.Close();

            ShowExifData(metaData.ExifMetaData);
        }

        private void ShowExifData(ExifMetaData exif)
        {
            var items = new List<ExifItem>();
            int i = 0;

            foreach (var ifd in exif.ImageFileDirectories)
            {
                AddIfd(ifd, items, $"IFD{i}");
                i++;
            }

            ExifGrid.ItemsSource = items;
        }

        private void AddIfd(Ifd ifd, List<ExifItem> items, string name)
        {
            if (ifd.IsEmpty())
            {
                return;
            }

            items.Add(new ExifItem($"---- {name} ----"));

            AddProps(ifd, items, name);
            AddIfd(ifd.ExifIfd, items, $"{name}:EXIF");
            AddIfd(ifd.GpsIfd, items, $"{name}:GPS");
            AddIfd(ifd.InteropIfd, items, $"{name}:Interop");

            int k = 0;

            foreach (var subIfdChain in ifd.SubIfds)
            {
                int i = 0;

                foreach (var subIfd in subIfdChain)
                {
                    AddIfd(subIfd, items, $"{name}:SubIFD{k}-{i}");
                    i++;
                }

                k++;
            }
        }

        private void AddProps(Ifd ifd, List<ExifItem> items, string name)
        {
            foreach (var prop in ifd.Properties)
            {
                items.Add(new ExifItem(prop.TagId.ToString(), prop.ToString()));
            }

            if (ifd.HasStrips())
            {
                ifd.GetStrips(out _, out var offsets, out var counts);
                items.Add(new ExifItem("StripCount", offsets.Length.ToString()));
                var sum = counts.Sum(x => x);
                items.Add(new ExifItem("StripByteCounts", sum.ToString()));
            }

            if (ifd.HasTiles())
            {
                ifd.GetTiles(out _, out var offsets, out var counts);
                items.Add(new ExifItem("TileCount", offsets.Length.ToString()));
                var sum = counts.Sum(x => x);
                items.Add(new ExifItem("TileByteCounts", sum.ToString()));
            }

            if (ifd.HasThumbnail())
            {
                ifd.GetThumbnail(out _, out var offset, out var count);
                items.Add(new ExifItem("ThumbnailOffset", offset.ToString()));
                items.Add(new ExifItem("ThumbnailLength", count.ToString()));
            }
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length != 1)
            {
                e.Effects = DragDropEffects.None;
            }
            else
            {
                e.Effects = DragDropEffects.Move;
            }

            e.Handled = true;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length != 1)
            {
                return;
            }

            Filename.Text = files[0];
            globalFilename = files[0];
            ShowExifData(globalFilename);
        }
        
        private void Button_MiscTest(object sender, RoutedEventArgs e)
        {
            FileStream sourceStream = File.OpenRead(globalFilename);
            var metaData = ExifDataFile.Load(sourceStream);

            var srcEx = metaData.ExifMetaData;
            var ifd0 = srcEx.ImageFileDirectories[0];
            var t1 = ifd0.GetProperty(TagId.Orientation);
            var t = ifd0.GetOrientation();

            try
            {
                ifd0.SetProperty(new UShortProperty(TagId.Orientation, 42));
            }
            catch (Exception ex)
            {
            }
        }
}

    /////////////////////////////////////////////////////////////

    public class ExifItem
    {
        public ExifItem(string prop)
        {
            Property = prop;
            Value = string.Empty;
        }

        public ExifItem(string prop, string value)
        {
            Property = prop;
            Value = value;
        }

        public string Property { get; set; }

        public string Value { get; set; }
    }
}
