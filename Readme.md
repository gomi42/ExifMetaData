# ExifMetaData

The **ExifMetaData** library lets you create, read (interpret) and write exif metadata from and to binary data. **ExifMetaData** offers read and write support of TIFF, JPG, PNG and WebP files.

### Type Safe
**ExifMetaData** is type safe. Strict type checks are build in and you cannot get around it. In case of strings **ExifMetaData** selects the correct string type for you (ASCII, UNICODE, multi-byte).

### Image File Directories
Exif metadata consist of a series of chained image file directories (short IFD). The IFD chain is represented as a list of IFDs. The `ExifMetaData` class exposes the property `ImageFileDirectories`.  `ImageFileDirectories` is the starting point of all operations. Create or select and IFD to get or set a property or remove an IFD.

### Properties
The implementation of **ExifMetaData** differs a little from similar implementations. The basic idea is that the API of an IFD exposes properties you work with. There several types of properties:

#### Single Type Property
A single type property holds one or multiple values of the same type. E.g. **Artist** consists of multiple characters, **LensSpecification** requires 4 **URationals**. 99.9% are single type properties. These properties are managed in `Ifd.Properties`.

**Important to note:** In contrast to similar implementations `Ifd.Properties` does not contain any administration data, only pure data properties, e.g. no information about used/linked sub-IFDs. You never work with or see such data, they are all managed internally by **ExifMetaData**.

#### Special sub-IFDs
The special sub-IFDs are managed via the `ExifIfd`, `GpsIfd`, `InteropIfd` and `SubIfds` properties.

#### Stripe
The **Stripe** information of a TIFF files is managed via
 ```
bool HasStrips()
void SetStrips(Stream stream, uint[] sourceOffset, uint[] counts)
bool GetStrips(out Stream stream, out uint[] sourceOffset, out uint[] counts)
 ```
#### Tile
The **Tile** information of a TIFF files is managed via
 ```
bool HasTiles()
bool GetTiles(out Stream stream, out uint[] sourceOffset, out uint[] counts)
void SetTiles(Stream stream, uint[] sourceOffset, uint[] counts)
```
#### Thumbnail
Thumbnails are managed via
```
bool HasThumbnail()
void GetThumbnail(out Stream sourceStream, out uint sourceOffset, out int count)
void SetThumbnail(Stream sourceStream, uint sourceOffset, int count)
```

### Tag ID
Properties in `Ifd.Properties` are managed by a **tag ID**. A tag ID is a handle that fully describes an exif tag; it combines all information about a tag like the tag number, which IFD the tag belongs to, the type(s) of the tag and more. **TagId.cs** contains all tag IDs.

### Special IFDs
An IFD may contain the predefined sub-IFDs **ExifIfd**, **GpsIfd**, **InteropIfd** and **SubIfds**. They play a special role that's why these three IFDs are properties of any standard IFD. There are 2 options of how to set and get tag IDs of these three special IFDs.

The special sub-IFD **SubIfds** can only be accessed via option 1.

#### Option 1
First select the parent IFD, next get the special IFD via the properties `ExifIfd`, `GpsIfd`, `InteropIfd` and `SubIfds` of the parent IFD. Work with that IFD like with all other IFD, there are no restrictions.

```
ExifMetaData exifMeta;
var ifd0 = exifMeta.ImageFileDirectories[0].
var exifIfd = ifd0.ExifIfd;
exifIfd.SetLensModel("My Lens");
```

#### Option 2
As mentioned the **ExifMetaData** library knows via the tag ID which IFD a tag belongs to: a standard IFD or one of the special IFDs (except **SubIfds**). **ExifMetaData** is smart enough to select the right sub-IFD for you in case you work with a standard (parent) IFD. The following code is equivalent to the code of option 1 but **ExifMetaData** automatically selects the correct IFD:

```
ExifMetaData exifMeta;
var ifd0 = exifMeta.ImageFileDirectories[0].
ifd0.SetLensModel("My Lens");
```

### Working With Single Type Properties

**ExifMetaData** offers 3 different access levels to the metadata.

#### Level 1
Level 1 gives you a very low level access. There are only the 2 methods `SetProperty` and `GetProperty`. Both methods require a tag ID. Additionally you need to know the type you want to set or get. All types follow the naming convention **xxxProperty** and inherit from `Property`.

```
void SetProperty(Property property)
Property GetProperty(TagId tagId)
bool PropertyExists(TagId tagId)
bool TryGetProperty(TagId tagId, out Property property)
bool RemoveProperty(TagId tagId)
```
Only level 1 offers methods to test and remove properties.

Example:
```
ifd.SetProperty(new UShortProperty(TagId.SamplesPerPixel, 3);
ifd.SetProperty(new StringProperty(TagId.Artist, "John Doe");
ifd.SetProperty(new OrientationProperty(TagId.Orientation, Orientation.Horizontal));

UShortProperty ushortProp = ifd.GetProperty(TagId.SamplesPerPixel);
ushort samplesPerPixel = ushortProp.Value;

StringProperty strProp = ifd.GetProperty(TagId.Artist);
string artist = strProp.Value;

OrientationProperty orientProp = ifd.GetProperty(TagId.Orientation);
orientation = orientProp.Value;
```

#### Level 2
Level 2 simplifies the access a bit and offers for each type its own access methods. Don't be confused, the methods refer to the type only, you still need to specify the tag ID. Level 2 methods follow the naming convention **GetxxxProperty** and **SetxxxProperty** while **xxxProperty** refers to the type as in level 1. Refer to **IfdProperties.cs** for all available methods.

```
ifd0.SetUShortProperty(TagId.SamplesPerPixel, 3);
ifd0.SetStringProperty(TagId.Artist, "John Doe");
ifd0.SetOrientationProperty(TagId.Orientation, Orientation.Horizontal);

ushort samplesPerPixel = ifd0.GetUShortProperty(TagId.SamplesPerPixel);
string artist = ifd0.GetStringProperty(TagId.Artist);
Orientation orientation = ifd0.GetOrientationProperty(TagId.Orientation);
```

#### Level 3
Level 3 combines all and offers for each tag ID its own get and set method. Refer to **UserExtensions.cs** for all available methods (almost 400).
```
ifd0.SetSamplesPerPixel(3);
ifd0.SetArtist("John Doe");
ifd0.SetOrientation(Orientation.Horizontal);

ushort samplesPerPixel = ifd0.GetSamplesPerPixel();
string artist = ifd0.GetArtist();
Orientation orientation = ifd0.GetOrientation();
```
### Deserialize Binary Metadata
The following example demonstrates how to deserialize binary exif metadata from a given stream. The position of the stream must be set to the beginning of the exif metadata (the TIFF header).
```
var reader = new ExifBinaryReader();
ExifMetaData exifMeta = reader.Read(stream);
```

### Create Metadata from Scratch

The following example demonstrates how to create EXIF metadata from scratch and serialize them into a byte array:

```
ExifMetaData exifMeta = new ExifMetaData();
IFD ifd0 = new IFD();
exifMeta.ImageFileDirectories.Add(ifd0);
// set all your data:
ifd0.SetArtist("John Doe");

var writer = ExifBinaryWriter(exifMeta);
byte[] exifBytes = writer.WriteBinary();
```
The following example is equivalent to `writer.WriteBinary()` but demonstrates how to serialize into a stream:

```
byte[] exifBinData;

using (var destStream = new MemoryStream())
{
    writer.Write(destStream);
    // optionally set the byte order
    writer.ByteOrder = ByteOrder.LittleEndian;
    exifBinData = destStream.ToArray();
}

```

### Build-in File Support
The build-in file support is an add-on and sits on top of **ExifMetaData**. The file support API could even be a separate project (I am still considering it). The following file formats are supported:

* TIFF
* JPG
* PNG
* WebP

You can read metadata from and write metadata to these files. **ExifMetaData** reads and writes the following metadata:

* EXIF (full editing with this library)
* XMP (no editing)
* IPTC (binary only)
* ICC Profile (binary only)

#### Read Metadata
One goal was a unified API for all file types that reads the 4 metadata. The following code demonstrates how to read the metadata with automatic file type selection:
```
FileStream sourceStream = File.OpenRead(filename);
// metaData contains all 4 types, EXIF as decoded ExifMetaData object
ImageMetaData metaData = ExifDataFile.Load(sourceStream);
sourceStream.Close();
```
The following code demonstrates how to read the metadata from a JPG file:
```
FileStream sourceStream = File.OpenRead(filename);
ImageMetaData metaData = ExifJpg.Load(sourceStream);
sourceStream.Close();
```

#### Write Metadata
Because of the different nature of the file types, a unified writing API is impossible. `ImageMetaDataWriteOption` is a superset of all settings of all types. It is not the best API but a starting point.

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

To add a display converter, implement it, add it to the tag ID in **AllTags.json** and finally run the code generator.

#### Enumerate all Tags
The test application implements the method `void ShowExifData(ExifMetaData exif)` that enumerates and displays the complete exif metadata tree in a datagrid.

#### Copy TIFF
The method `Button_Click_CopyTiff` demonstrates how to copy a tiff file. The tiff isn't copied here using the build-in support. A new **ExifMetaData** object is created and only the required TIFF properties are coped over. It creates a bare metal tiff file from scratch.

### Internals
#### Code Generator
The code relies heavyly on automatic code generation to get the huge amount of tag IDs and enum types under control. The heard are the 2 files **AllTags.json** and **AllTypes.json**. **t4** templates could habe been one approach to transform both files into the 6 .cs files. It is still on my radar. But because of the somewhat cryptic t4 files, development (including debugging) of a stand-alone code generator app was way easier.

#### How to Add an Enum
Some enums are already implemented like **Orientation**. Steps to add an enum:

1. Add the enum to **Enums.cs**
1. Add an enum entry to **AllTypes.json**. Pay attention to `EnumBase`. Its usually `UShort`, but few exceptions require `ULong`.
1. Set the `class` of the tag ID in **AllTags.json** to the name of the enum.
1. Run the code generator.

#### Implementation

One design goal was that the developer does not need to care about exif admininstration data. As a result `Ifd.Properties` does not contain (and even cannot store) administration data. The **ExifBinaryWriter** creates a mirror tag tree of all properties and inserts the necessary administration tags into the mirror in the correct order. The mirror then is rendered to binary. `Ifd.Properties` is kept unchanged.

Rendering an exif tree requires a lot of look-ahead information. The mirror tree offers an easy way to get the look-ahead information before rendering the data. The render process is a two pass process. The first pass is a dry run. It collects all look-ahead information and adds them to the mirror tree. The second pass renders the tree in one go without changing the stream position and patching some data afterwards.

The mirror tree offers even more features. The EXIF definitions requires an exif version tag in the sub-ExifIFD. In case you do not set the version, **ExifMetaData** does it for you in the mirror tree. The `Properties` are kept untouched.

#### Idea
It would be an easy enhancement to support multiple streams for TIFF stripes and tiles. E.g. one stream for each stripe/tile. Currently, only a single stream is supported (which is not a restriction at the moment).