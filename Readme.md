# ExifMetaData

The **ExifMetaData** library lets you read (interpret) and write exif metadata from and to binary data.

**ExifMetaData** defines a `tag ID`. It is a handle that fully describes a tag; it combines all information about a tag like the tag number, which IFD the tag belongs to, the type(s) of the tag.

Exif metadata consist of image file directories (short IFD). IDFs are chained. This IFD chain is represented in the list of IFDs property `ImageFileDirectories` of the `ExifMetaData` class. When you want to get or set a tag ID you need to select or create an IFD first.

### Special IFDs
An IFD may contain the predefined sub-IFDs **ExifIfd**, **GpsIfd**, **InteropIfd**. They play a special role that's why these three IFDs are properties of any standard IFD. There are 2 options of how to set and get tag IDs of these three special IFDs:

#### Option 1
First select the parent IFD, next get the special IFD via the properties `ExifIfd`, `GpsIfd` and `InteropIfd` of the parent IFD. Work with that IFD like with all other IFD, there are not restrictions.

```
ExifMetaData exifMeta;
var ifd0 = exifMeta.ImageFileDirectories[0].
var exifIfd = ifd0.ExifIfd;
exifIfd.SetLensModel("My Lens");
```

#### Option 2
As mentioned the **ExifMetaData** library knows via a tag ID which IFD a tag is to be written to: a standard IFD or one of the special IFDs. **ExifMetaData** is smart enough to select the right sub-IFD for you in case you work with a standard (parent) IFD. The following code is equivalent to the code of option 1 but it lets **ExifMetaData** select the correct IFD:

```
ExifMetaData exifMeta;
var ifd0 = exifMeta.ImageFileDirectories[0].
ifd0.SetLensModel("My Lens");
```


### Working With Data

The **ExifMetaData** library offers 3 different levels of access to the metadata.

#### Level 1
Level 1 gives you a very low level access. There are only the 2 methods `SetProperty` and `GetProperty`. Both methods require a tag ID. Additionally you need to know the type you want to set or get. All types follow the naming convention **xxxProperty**.

```
ifd.SetProperty(new StringProperty(TagId.Artist, "John Doe");
ifd.SetProperty(new OrientationProperty(TagId.Orientation, Orientation.Horizontal));

StringProperty strProp = ifd.GetProperty(TagId.Artist);
string artist = strProp.Value;

OrientationProperty orientProp = ifd.GetProperty(TagId.Orientation);
orientation = orientProp.Value;
```

#### Level 2
Level 2 simplifies the access a bit but offers for each type its own access methods. Don't be confused, the methods refer to type only, you still need to specify which tag ID to get or set. Level 2 methods follow follow the naming convention **GetxxxProperty** and **SetxxxProperty** while **xxxProperty** refers to the type as in level 1.

```
ifd.SetStringProperty(TagId.Artist, "John Doe");
ifd.SetOrientationProperty(TagId.Orientation, Orientation.Horizontal);

string artist = ifd.GetStringProperty(TagId.Artist);
Orientation orientation = ifd.GetOrientationProperty(TagId.Orientation);
```

#### Level 3
Level 3 combines all and offers for each tag ID is own get and set method.
```
ifd.SetArtist("John Doe");
ifd.SetOrientation(Orientation.Horizontal);

string artist = ifd.GetArtist();
Orientation orientation = ifd.GetOrientation();
```

### Create Metadata from Scratch

In case you need the exif metadata as binary data in memory (or)

```
ExifMetaData exifMeta = new ExifMetaData();
IFD ifd0 = new IFD();
exifMeta.ImageFileDirectories.Add(ifd0);
// set all your data like:
ifd0.SetArtist("John Doe");

var writer = ExifBinaryWriter(exifMeta);
byte[] exifBinData;

using (var destStream = new MemoryStream())
{
    writer.WriteAllWithTiffHeader(destStream);
    exifBinData = destStream.ToArray();
}
```

### Build in File Support
**ExifMetaData** supports the following file formats:

* TIFF
* JPG
* PNG
* WebP

You can read metadata from and write metadata to these files. In this case **ExifMetaData** reads and writes the following metadata:

* EXIF
* XMP (no editing)
* IPTC (binary only)
* ICC Profile (binary only)

#### Read Metadata
```
FileStream sourceStream = File.OpenRead(filename);
// metaData contains all 4 types, EXIF as decoded ExifMetaData object
var metaData = ExifDataFile.Load(sourceStream);
sourceStream.Close();
```

#### Write Metadata
Writing metadata is as simple as reading them. The writer supports:
* keep the original data
* overwrite the data with new data
* remove the data

```
FileStream sourceStream = File.OpenRead(filename);
var metaData = ExifDataFile.Load(sourceStream);

var mdw = new ImageMetaDataWrite();

mdw.ExifMetaData = metaData.ExifMetaData;
mdw.ExifMetaDataOption = metaData.ExifMetaData != null ? ImageMetaDataWriteOption.Overwrite : ImageMetaDataWriteOption.KeepOriginal;

// modify the exif data
// ...

mdw.Xmp = metaData.Xmp;
mdw.XmpOption = metaData.Xmp != null ? ImageMetaDataWriteOption.Overwrite : ImageMetaDataWriteOption.KeepOriginal;
var xmpString = Encoding.UTF8.GetString(metaData.Xmp);

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

var destStream = File.Create(GetCopyFilename(filename));
ExifDataFile.Save(sourceStream, destStream, mdw);

sourceStream.Close();
destStream.Close();
```

### Future
Let me know if there are tag IDs missing.

Not all enum types are coded right now. I'll add them on demand.

### Tips and Tricks
#### Display
The main intention of **ExifMetaData** is to create, read and/or metadata in binary form. There is limited support to display the content of a tag. All classes **xxxProperty** implement `ToString()` that tries its best. But it is not suitable for production user interfaces. **ExifMetaData** could be enhanced to support that but again, its not the main intention. The `IDisplayConverter` implementations demonstrate how to add a unit or how to display human readable values out of apex values.
#### Enumerate all Tags
The test application implements the method `void ShowExifData(ExifMetaData exif)` that enumerates and displays the complete exif metadata tree in a datagrid.

The method `Button_Click_CopyTiff` demonstrates how to copy a tiff file. The tiff isn't copied here using the build-in support. A new **ExifMetaData** object is created and only the needed properties are coped over. It creates a bare metal tiff file from scratch.

### Internals
#### Code Generator
The code relies heavyly on automatic code generation to get the huge amount of tag IDs, enums type under control. The heard are the 2 files **AllTags.json** and **AllTypes.json**. **t4** templates could habe been one approach to transform both files into the 6 .cs files. It is still on my radar. But because of the somewhat cryptic t4 files, development (including debugging) of a stand-alone code generator app was way easier.

#### How to Add an Enum
Some enums are already implemented like **Orientation**. Steps to add an enum:

1. Add the enum to **Enums.cs**
1. Add an enum entry to **AllTypes.json**. Pay attention to `EnumBase`. Its usually `UShort`, but few exceptions require `ULong`.
1. Set the `class` of the tag ID in **AllTags.json** to the name of the enum.
1. Run the code generator.

#### Idea
It would be an easy enhancement to support multiple streams for tiff stripes/tiles. One stream for each stripe/tile. Currently, only a single stream is supported (which is not a restriction at the moment).