// Automatically generated file. Do not modify.

using System.Collections.Generic;

namespace ExifMeta
{
    internal static partial class TagDetailsRegistry
    {
        private static Dictionary<TagId, TagDetails> tagDetails = new Dictionary<TagId, TagDetails>()
        {
            {
                TagId.NewSubfileType,
                new TagDetails(IfdId.Standard,
                               Tag.NewSubfileType,
                               DataType.ULong,
                               false,
                               false,
                               typeof(SubFileTypeProperty))
            },
            {
                TagId.SubfileType,
                new TagDetails(IfdId.Standard,
                               Tag.SubfileType,
                               DataType.UShort,
                               false,
                               false,
                               typeof(UShortProperty))
            },
            {
                TagId.ImageWidth,
                new TagDetails(IfdId.Standard,
                               Tag.ImageWidth,
                               new[] { DataType.UShort, DataType.ULong },
                               false,
                               false,
                               typeof(UShortUIntProperty))
            },
            {
                TagId.ImageHeight,
                new TagDetails(IfdId.Standard,
                               Tag.ImageHeight,
                               new[] { DataType.UShort, DataType.ULong },
                               false,
                               false,
                               typeof(UShortUIntProperty))
            },
            {
                TagId.BitsPerSample,
                new TagDetails(IfdId.Standard,
                               Tag.BitsPerSample,
                               DataType.UShort,
                               false,
                               false,
                               typeof(UShortProperty),
                               -1)
            },
            {
                TagId.Compression,
                new TagDetails(IfdId.Standard,
                               Tag.Compression,
                               DataType.UShort,
                               false,
                               false,
                               typeof(CompressionProperty))
            },
            {
                TagId.PhotometricInterpretation,
                new TagDetails(IfdId.Standard,
                               Tag.PhotometricInterpretation,
                               DataType.UShort,
                               false,
                               false,
                               typeof(PhotometricInterpretationProperty))
            },
            {
                TagId.Threshholding,
                new TagDetails(IfdId.Standard,
                               Tag.Threshholding,
                               DataType.UShort,
                               false,
                               false,
                               typeof(UShortProperty))
            },
            {
                TagId.CellWidth,
                new TagDetails(IfdId.Standard,
                               Tag.CellWidth,
                               DataType.ULong,
                               false,
                               false,
                               typeof(UIntProperty))
            },
            {
                TagId.CellLength,
                new TagDetails(IfdId.Standard,
                               Tag.CellLength,
                               DataType.ULong,
                               false,
                               false,
                               typeof(UIntProperty))
            },
            {
                TagId.FillOrder,
                new TagDetails(IfdId.Standard,
                               Tag.FillOrder,
                               DataType.ULong,
                               false,
                               false,
                               typeof(UIntProperty))
            },
            {
                TagId.DocumentName,
                new TagDetails(IfdId.Standard,
                               Tag.DocumentName,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               -1)
            },
            {
                TagId.ImageDescription,
                new TagDetails(IfdId.Standard,
                               Tag.ImageDescription,
                               new[] { DataType.Ascii, DataType.Utf8 },
                               true,
                               false,
                               typeof(StringUtf8Property),
                               -1)
            },
            {
                TagId.Make,
                new TagDetails(IfdId.Standard,
                               Tag.Make,
                               new[] { DataType.Ascii, DataType.Utf8 },
                               true,
                               false,
                               typeof(StringUtf8Property),
                               -1)
            },
            {
                TagId.Model,
                new TagDetails(IfdId.Standard,
                               Tag.Model,
                               new[] { DataType.Ascii, DataType.Utf8 },
                               true,
                               false,
                               typeof(StringUtf8Property),
                               -1)
            },
            {
                TagId.StripOffsets,
                new TagDetails(IfdId.Standard,
                               Tag.StripOffsets,
                               new[] { DataType.ULong, DataType.UShort },
                               false,
                               true,
                               typeof(UShortUIntProperty),
                               -1)
            },
            {
                TagId.Orientation,
                new TagDetails(IfdId.Standard,
                               Tag.Orientation,
                               DataType.UShort,
                               false,
                               false,
                               typeof(OrientationProperty))
            },
            {
                TagId.SamplesPerPixel,
                new TagDetails(IfdId.Standard,
                               Tag.SamplesPerPixel,
                               DataType.UShort,
                               false,
                               false,
                               typeof(UShortProperty))
            },
            {
                TagId.RowsPerStrip,
                new TagDetails(IfdId.Standard,
                               Tag.RowsPerStrip,
                               new[] { DataType.UShort, DataType.ULong },
                               false,
                               false,
                               typeof(UShortUIntProperty))
            },
            {
                TagId.StripByteCounts,
                new TagDetails(IfdId.Standard,
                               Tag.StripByteCounts,
                               new[] { DataType.ULong, DataType.UShort },
                               false,
                               true,
                               typeof(UShortUIntProperty),
                               -1)
            },
            {
                TagId.MinSampleValue,
                new TagDetails(IfdId.Standard,
                               Tag.MinSampleValue,
                               DataType.UShort,
                               false,
                               false,
                               typeof(UShortProperty))
            },
            {
                TagId.MaxSampleValue,
                new TagDetails(IfdId.Standard,
                               Tag.MaxSampleValue,
                               DataType.UShort,
                               false,
                               false,
                               typeof(UShortProperty))
            },
            {
                TagId.XResolution,
                new TagDetails(IfdId.Standard,
                               Tag.XResolution,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty))
            },
            {
                TagId.YResolution,
                new TagDetails(IfdId.Standard,
                               Tag.YResolution,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty))
            },
            {
                TagId.PlanarConfiguration,
                new TagDetails(IfdId.Standard,
                               Tag.PlanarConfiguration,
                               DataType.UShort,
                               false,
                               false,
                               typeof(PlanarConfigurationProperty))
            },
            {
                TagId.PageName,
                new TagDetails(IfdId.Standard,
                               Tag.PageName,
                               DataType.Ascii,
                               false,
                               false,
                               typeof(StringProperty),
                               -1)
            },
            {
                TagId.XPosition,
                new TagDetails(IfdId.Standard,
                               Tag.XPosition,
                               DataType.URational,
                               false,
                               false,
                               typeof(URationalProperty))
            },
            {
                TagId.YPosition,
                new TagDetails(IfdId.Standard,
                               Tag.YPosition,
                               DataType.URational,
                               false,
                               false,
                               typeof(URationalProperty))
            },
            {
                TagId.FreeOffsets,
                new TagDetails(IfdId.Standard,
                               Tag.FreeOffsets,
                               DataType.ULong,
                               false,
                               true,
                               typeof(UIntProperty))
            },
            {
                TagId.FreeByteCounts,
                new TagDetails(IfdId.Standard,
                               Tag.FreeByteCounts,
                               DataType.ULong,
                               false,
                               true,
                               typeof(UIntProperty))
            },
            {
                TagId.GrayResponseUnit,
                new TagDetails(IfdId.Standard,
                               Tag.GrayResponseUnit,
                               DataType.UShort,
                               false,
                               false,
                               typeof(UShortProperty))
            },
            {
                TagId.GrayResponseCurve,
                new TagDetails(IfdId.Standard,
                               Tag.GrayResponseCurve,
                               DataType.UShort,
                               false,
                               false,
                               typeof(UShortProperty),
                               -1)
            },
            {
                TagId.T4Options,
                new TagDetails(IfdId.Standard,
                               Tag.T4Options,
                               DataType.ULong,
                               false,
                               false,
                               typeof(UIntProperty))
            },
            {
                TagId.T6Options,
                new TagDetails(IfdId.Standard,
                               Tag.T6Options,
                               DataType.ULong,
                               false,
                               false,
                               typeof(UIntProperty))
            },
            {
                TagId.ResolutionUnit,
                new TagDetails(IfdId.Standard,
                               Tag.ResolutionUnit,
                               DataType.UShort,
                               false,
                               false,
                               typeof(LengthUnitProperty))
            },
            {
                TagId.PageNumber,
                new TagDetails(IfdId.Standard,
                               Tag.PageNumber,
                               DataType.UShort,
                               false,
                               false,
                               typeof(UShortProperty),
                               2)
            },
            {
                TagId.TransferFunction,
                new TagDetails(IfdId.Standard,
                               Tag.TransferFunction,
                               DataType.UShort,
                               false,
                               false,
                               typeof(UShortProperty),
                               768)
            },
            {
                TagId.Software,
                new TagDetails(IfdId.Standard,
                               Tag.Software,
                               new[] { DataType.Ascii, DataType.Utf8 },
                               true,
                               false,
                               typeof(StringUtf8Property),
                               -1)
            },
            {
                TagId.DateTime,
                new TagDetails(IfdId.Standard,
                               Tag.DateTime,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(DateTimeProperty),
                               -1)
            },
            {
                TagId.Artist,
                new TagDetails(IfdId.Standard,
                               Tag.Artist,
                               new[] { DataType.Ascii, DataType.Utf8 },
                               true,
                               false,
                               typeof(StringUtf8Property),
                               -1)
            },
            {
                TagId.HostComputer,
                new TagDetails(IfdId.Standard,
                               Tag.HostComputer,
                               DataType.Ascii,
                               false,
                               false,
                               typeof(StringProperty),
                               -1)
            },
            {
                TagId.Predictor,
                new TagDetails(IfdId.Standard,
                               Tag.Predictor,
                               DataType.UShort,
                               false,
                               false,
                               typeof(PredictorProperty))
            },
            {
                TagId.WhitePoint,
                new TagDetails(IfdId.Standard,
                               Tag.WhitePoint,
                               DataType.URational,
                               false,
                               false,
                               typeof(URationalProperty),
                               2)
            },
            {
                TagId.PrimaryChromaticities,
                new TagDetails(IfdId.Standard,
                               Tag.PrimaryChromaticities,
                               DataType.URational,
                               false,
                               false,
                               typeof(URationalProperty),
                               6)
            },
            {
                TagId.ColorMap,
                new TagDetails(IfdId.Standard,
                               Tag.ColorMap,
                               DataType.UShort,
                               false,
                               false,
                               typeof(UShortProperty))
            },
            {
                TagId.HalftoneHints,
                new TagDetails(IfdId.Standard,
                               Tag.HalftoneHints,
                               DataType.UShort,
                               false,
                               false,
                               typeof(UShortProperty),
                               2)
            },
            {
                TagId.TileWidth,
                new TagDetails(IfdId.Standard,
                               Tag.TileWidth,
                               DataType.ULong,
                               false,
                               false,
                               typeof(UIntProperty))
            },
            {
                TagId.TileLength,
                new TagDetails(IfdId.Standard,
                               Tag.TileLength,
                               DataType.ULong,
                               false,
                               false,
                               typeof(UIntProperty))
            },
            {
                TagId.TileOffsets,
                new TagDetails(IfdId.Standard,
                               Tag.TileOffsets,
                               new[] { DataType.ULong, DataType.UShort },
                               false,
                               true,
                               typeof(UShortUIntProperty),
                               -1)
            },
            {
                TagId.TileByteCounts,
                new TagDetails(IfdId.Standard,
                               Tag.TileByteCounts,
                               new[] { DataType.ULong, DataType.UShort },
                               false,
                               true,
                               typeof(UShortUIntProperty),
                               -1)
            },
            {
                TagId.SubIFDs,
                new TagDetails(IfdId.Standard,
                               Tag.SubIFDs,
                               new[] { DataType.Ifd, DataType.ULong },
                               false,
                               false,
                               null)
            },
            {
                TagId.InkSet,
                new TagDetails(IfdId.Standard,
                               Tag.InkSet,
                               DataType.UShort,
                               false,
                               false,
                               typeof(UShortProperty))
            },
            {
                TagId.InkNames,
                new TagDetails(IfdId.Standard,
                               Tag.InkNames,
                               DataType.Ascii,
                               false,
                               false,
                               typeof(StringProperty),
                               -1)
            },
            {
                TagId.NumberOfInks,
                new TagDetails(IfdId.Standard,
                               Tag.NumberOfInks,
                               DataType.UShort,
                               false,
                               false,
                               typeof(UShortProperty))
            },
            {
                TagId.DotRange,
                new TagDetails(IfdId.Standard,
                               Tag.DotRange,
                               DataType.UShort,
                               false,
                               false,
                               typeof(UShortProperty))
            },
            {
                TagId.TargetPrinter,
                new TagDetails(IfdId.Standard,
                               Tag.TargetPrinter,
                               DataType.Ascii,
                               false,
                               false,
                               typeof(StringProperty),
                               -1)
            },
            {
                TagId.ExtraSamples,
                new TagDetails(IfdId.Standard,
                               Tag.ExtraSamples,
                               DataType.UShort,
                               false,
                               false,
                               typeof(UShortProperty))
            },
            {
                TagId.SampleFormat,
                new TagDetails(IfdId.Standard,
                               Tag.SampleFormat,
                               DataType.UShort,
                               false,
                               false,
                               typeof(UShortProperty),
                               -1)
            },
            {
                TagId.SMinSampleValue,
                new TagDetails(IfdId.Standard,
                               Tag.SMinSampleValue,
                               new[] { DataType.UShort, DataType.ULong },
                               false,
                               false,
                               typeof(UShortUIntProperty))
            },
            {
                TagId.SMaxSampleValue,
                new TagDetails(IfdId.Standard,
                               Tag.SMaxSampleValue,
                               new[] { DataType.UShort, DataType.ULong },
                               false,
                               false,
                               typeof(UShortUIntProperty))
            },
            {
                TagId.TransferRange,
                new TagDetails(IfdId.Standard,
                               Tag.TransferRange,
                               new[] { DataType.UShort, DataType.ULong },
                               false,
                               false,
                               typeof(UShortUIntProperty))
            },
            {
                TagId.JpegTabels,
                new TagDetails(IfdId.Standard,
                               Tag.JpegTabels,
                               DataType.Byte,
                               false,
                               false,
                               typeof(ByteProperty),
                               -1)
            },
            {
                TagId.GlobalParametersIFD,
                new TagDetails(IfdId.Standard,
                               Tag.GlobalParametersIFD,
                               DataType.ULong,
                               false,
                               false,
                               typeof(UIntProperty))
            },
            {
                TagId.JpegProc,
                new TagDetails(IfdId.Standard,
                               Tag.JpegProc,
                               DataType.ULong,
                               false,
                               false,
                               typeof(UIntProperty))
            },
            {
                TagId.JpegInterchangeFormat,
                new TagDetails(IfdId.Standard,
                               Tag.JpegInterchangeFormat,
                               DataType.ULong,
                               false,
                               true,
                               typeof(UIntProperty))
            },
            {
                TagId.JpegInterchangeFormatLength,
                new TagDetails(IfdId.Standard,
                               Tag.JpegInterchangeFormatLength,
                               DataType.ULong,
                               false,
                               true,
                               typeof(UIntProperty))
            },
            {
                TagId.JpegRestartInterval,
                new TagDetails(IfdId.Standard,
                               Tag.JpegRestartInterval,
                               DataType.ULong,
                               false,
                               false,
                               typeof(UIntProperty))
            },
            {
                TagId.JpegLosslessPredictors,
                new TagDetails(IfdId.Standard,
                               Tag.JpegLosslessPredictors,
                               DataType.ULong,
                               false,
                               false,
                               typeof(UIntProperty))
            },
            {
                TagId.JpegPointTransforms,
                new TagDetails(IfdId.Standard,
                               Tag.JpegPointTransforms,
                               DataType.ULong,
                               false,
                               false,
                               typeof(UIntProperty))
            },
            {
                TagId.JpegQTabels,
                new TagDetails(IfdId.Standard,
                               Tag.JpegQTabels,
                               DataType.Undefined,
                               false,
                               false,
                               typeof(BinaryProperty),
                               -1)
            },
            {
                TagId.JpegDCTables,
                new TagDetails(IfdId.Standard,
                               Tag.JpegDCTables,
                               DataType.Undefined,
                               false,
                               false,
                               typeof(BinaryProperty),
                               -1)
            },
            {
                TagId.JpegACTables,
                new TagDetails(IfdId.Standard,
                               Tag.JpegACTables,
                               DataType.Undefined,
                               false,
                               false,
                               typeof(BinaryProperty),
                               -1)
            },
            {
                TagId.YCbCrCoefficients,
                new TagDetails(IfdId.Standard,
                               Tag.YCbCrCoefficients,
                               DataType.URational,
                               false,
                               false,
                               typeof(URationalProperty),
                               3)
            },
            {
                TagId.YCbCrSubSampling,
                new TagDetails(IfdId.Standard,
                               Tag.YCbCrSubSampling,
                               DataType.UShort,
                               false,
                               false,
                               typeof(UShortProperty),
                               2)
            },
            {
                TagId.YCbCrPositioning,
                new TagDetails(IfdId.Standard,
                               Tag.YCbCrPositioning,
                               DataType.UShort,
                               false,
                               false,
                               typeof(UShortProperty))
            },
            {
                TagId.ReferenceBlackWhite,
                new TagDetails(IfdId.Standard,
                               Tag.ReferenceBlackWhite,
                               DataType.URational,
                               false,
                               false,
                               typeof(URationalProperty),
                               6)
            },
            {
                TagId.XmpMetadata,
                new TagDetails(IfdId.Standard,
                               Tag.XmpMetadata,
                               DataType.Byte,
                               false,
                               false,
                               typeof(ByteProperty),
                               -1)
            },
            {
                TagId.Copyright,
                new TagDetails(IfdId.Standard,
                               Tag.Copyright,
                               new[] { DataType.Ascii, DataType.Utf8 },
                               true,
                               false,
                               typeof(StringUtf8Property),
                               -1)
            },
            {
                TagId.IptcMetadata,
                new TagDetails(IfdId.Standard,
                               Tag.IptcMetadata,
                               new[] { DataType.ULong, DataType.Undefined },
                               false,
                               false,
                               typeof(BinaryProperty),
                               -1)
            },
            {
                TagId.PhotoshopSettings,
                new TagDetails(IfdId.Standard,
                               Tag.PhotoshopSettings,
                               DataType.Byte,
                               false,
                               true,
                               typeof(ByteProperty),
                               -1)
            },
            {
                TagId.ExifIfdPointer,
                new TagDetails(IfdId.Standard,
                               Tag.ExifIfdPointer,
                               new[] { DataType.ULong, DataType.Ifd },
                               false,
                               true,
                               typeof(UIntProperty))
            },
            {
                TagId.InterColorProfile,
                new TagDetails(IfdId.Standard,
                               Tag.InterColorProfile,
                               new[] { DataType.ULong, DataType.Undefined },
                               false,
                               false,
                               typeof(BinaryProperty),
                               -1)
            },
            {
                TagId.GpsIfdPointer,
                new TagDetails(IfdId.Standard,
                               Tag.GpsIfdPointer,
                               new[] { DataType.ULong, DataType.Ifd },
                               false,
                               true,
                               typeof(UIntProperty))
            },
            {
                TagId.TIFFEPStandardID,
                new TagDetails(IfdId.Standard,
                               Tag.TIFFEPStandardID,
                               DataType.Byte,
                               false,
                               false,
                               typeof(ByteProperty),
                               4)
            },
            {
                TagId.ImageSourceData,
                new TagDetails(IfdId.Standard,
                               Tag.ImageSourceData,
                               DataType.Undefined,
                               false,
                               true,
                               typeof(BinaryExternProperty),
                               -1)
                               { DontLoadPayload = true }
            },
            {
                TagId.XpTitle,
                new TagDetails(IfdId.Standard,
                               Tag.XpTitle,
                               DataType.Byte,
                               true,
                               false,
                               typeof(StringXpProperty),
                               -1)
            },
            {
                TagId.XpComment,
                new TagDetails(IfdId.Standard,
                               Tag.XpComment,
                               DataType.Byte,
                               true,
                               false,
                               typeof(StringXpProperty),
                               -1)
            },
            {
                TagId.XpAuthor,
                new TagDetails(IfdId.Standard,
                               Tag.XpAuthor,
                               DataType.Byte,
                               true,
                               false,
                               typeof(StringXpProperty),
                               -1)
            },
            {
                TagId.XpKeywords,
                new TagDetails(IfdId.Standard,
                               Tag.XpKeywords,
                               DataType.Byte,
                               true,
                               false,
                               typeof(StringXpProperty),
                               -1)
            },
            {
                TagId.XpSubject,
                new TagDetails(IfdId.Standard,
                               Tag.XpSubject,
                               DataType.Byte,
                               true,
                               false,
                               typeof(StringXpProperty),
                               -1)
            },
            {
                TagId.ExposureTime,
                new TagDetails(IfdId.Exif,
                               Tag.ExposureTime,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty))
                               { DisplayConverter = new ExposureTimeConverter() }
            },
            {
                TagId.FNumber,
                new TagDetails(IfdId.Exif,
                               Tag.FNumber,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty))
            },
            {
                TagId.ExposureProgram,
                new TagDetails(IfdId.Exif,
                               Tag.ExposureProgram,
                               DataType.UShort,
                               true,
                               false,
                               typeof(ExposureProgramProperty))
            },
            {
                TagId.SpectralSensitivity,
                new TagDetails(IfdId.Exif,
                               Tag.SpectralSensitivity,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               -1)
            },
            {
                TagId.PhotographicSensitivity,
                new TagDetails(IfdId.Exif,
                               Tag.PhotographicSensitivity,
                               DataType.UShort,
                               true,
                               false,
                               typeof(UShortProperty))
            },
            {
                TagId.Oecf,
                new TagDetails(IfdId.Exif,
                               Tag.Oecf,
                               DataType.Undefined,
                               false,
                               false,
                               typeof(BinaryProperty),
                               -1)
            },
            {
                TagId.SensitivityType,
                new TagDetails(IfdId.Exif,
                               Tag.SensitivityType,
                               DataType.UShort,
                               true,
                               false,
                               typeof(SensitivityTypeProperty))
            },
            {
                TagId.StandardOutputSensitivity,
                new TagDetails(IfdId.Exif,
                               Tag.StandardOutputSensitivity,
                               DataType.ULong,
                               true,
                               false,
                               typeof(UIntProperty))
            },
            {
                TagId.RecommendedExposureIndex,
                new TagDetails(IfdId.Exif,
                               Tag.RecommendedExposureIndex,
                               DataType.ULong,
                               true,
                               false,
                               typeof(UIntProperty))
            },
            {
                TagId.IsoSpeed,
                new TagDetails(IfdId.Exif,
                               Tag.IsoSpeed,
                               DataType.ULong,
                               true,
                               false,
                               typeof(UIntProperty))
            },
            {
                TagId.IsoSpeedLatitudeyyy,
                new TagDetails(IfdId.Exif,
                               Tag.IsoSpeedLatitudeyyy,
                               DataType.ULong,
                               true,
                               false,
                               typeof(UIntProperty))
            },
            {
                TagId.IsoSpeedLatitudezzz,
                new TagDetails(IfdId.Exif,
                               Tag.IsoSpeedLatitudezzz,
                               DataType.ULong,
                               true,
                               false,
                               typeof(UIntProperty))
            },
            {
                TagId.ExifVersion,
                new TagDetails(IfdId.Exif,
                               Tag.ExifVersion,
                               DataType.Undefined,
                               true,
                               false,
                               typeof(FourCharProperty),
                               -1)
            },
            {
                TagId.DateTimeOriginal,
                new TagDetails(IfdId.Exif,
                               Tag.DateTimeOriginal,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(DateTimeProperty),
                               -1)
            },
            {
                TagId.DateTimeDigitized,
                new TagDetails(IfdId.Exif,
                               Tag.DateTimeDigitized,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(DateTimeProperty),
                               -1)
            },
            {
                TagId.OffsetTime,
                new TagDetails(IfdId.Exif,
                               Tag.OffsetTime,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(TimeOffsetProperty),
                               -1)
            },
            {
                TagId.OffsetTimeOriginal,
                new TagDetails(IfdId.Exif,
                               Tag.OffsetTimeOriginal,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(TimeOffsetProperty),
                               -1)
            },
            {
                TagId.OffsetTimeDigitized,
                new TagDetails(IfdId.Exif,
                               Tag.OffsetTimeDigitized,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(TimeOffsetProperty),
                               -1)
            },
            {
                TagId.ComponentsConfiguration,
                new TagDetails(IfdId.Exif,
                               Tag.ComponentsConfiguration,
                               DataType.Undefined,
                               true,
                               false,
                               typeof(BinaryProperty),
                               -1)
            },
            {
                TagId.CompressedBitsPerPixel,
                new TagDetails(IfdId.Exif,
                               Tag.CompressedBitsPerPixel,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty))
            },
            {
                TagId.ShutterSpeedValue,
                new TagDetails(IfdId.Exif,
                               Tag.ShutterSpeedValue,
                               DataType.SRational,
                               true,
                               false,
                               typeof(Apex1Property))
            },
            {
                TagId.ApertureValue,
                new TagDetails(IfdId.Exif,
                               Tag.ApertureValue,
                               DataType.URational,
                               true,
                               false,
                               typeof(Apex2Property))
            },
            {
                TagId.BrightnessValue,
                new TagDetails(IfdId.Exif,
                               Tag.BrightnessValue,
                               DataType.SRational,
                               true,
                               false,
                               typeof(SRationalProperty))
            },
            {
                TagId.ExposureCompensation,
                new TagDetails(IfdId.Exif,
                               Tag.ExposureCompensation,
                               DataType.SRational,
                               true,
                               false,
                               typeof(SRationalProperty))
                               { DisplayConverter = new ExposureCompensationConverter() }
            },
            {
                TagId.MaxApertureValue,
                new TagDetails(IfdId.Exif,
                               Tag.MaxApertureValue,
                               DataType.URational,
                               true,
                               false,
                               typeof(Apex2Property))
            },
            {
                TagId.SubjectDistance,
                new TagDetails(IfdId.Exif,
                               Tag.SubjectDistance,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty))
            },
            {
                TagId.MeteringMode,
                new TagDetails(IfdId.Exif,
                               Tag.MeteringMode,
                               DataType.UShort,
                               true,
                               false,
                               typeof(MeteringModeProperty))
            },
            {
                TagId.LightSource,
                new TagDetails(IfdId.Exif,
                               Tag.LightSource,
                               DataType.UShort,
                               true,
                               false,
                               typeof(UShortProperty))
            },
            {
                TagId.Flash,
                new TagDetails(IfdId.Exif,
                               Tag.Flash,
                               DataType.UShort,
                               true,
                               false,
                               typeof(FlashProperty))
            },
            {
                TagId.FocalLength,
                new TagDetails(IfdId.Exif,
                               Tag.FocalLength,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty))
                               { DisplayConverter = new AddMmUnitConverter() }
            },
            {
                TagId.SubjectArea,
                new TagDetails(IfdId.Exif,
                               Tag.SubjectArea,
                               DataType.UShort,
                               true,
                               false,
                               typeof(UShortProperty),
                               -1)
            },
            {
                TagId.MakerNote,
                new TagDetails(IfdId.Exif,
                               Tag.MakerNote,
                               DataType.Undefined,
                               false,
                               false,
                               null,
                               -1)
            },
            {
                TagId.UserComment,
                new TagDetails(IfdId.Exif,
                               Tag.UserComment,
                               DataType.Undefined,
                               true,
                               false,
                               typeof(MulticodeUnicodeProperty),
                               -1)
            },
            {
                TagId.SubsecTime,
                new TagDetails(IfdId.Exif,
                               Tag.SubsecTime,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               -1)
            },
            {
                TagId.SubsecTimeOriginal,
                new TagDetails(IfdId.Exif,
                               Tag.SubsecTimeOriginal,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               -1)
            },
            {
                TagId.SubsecTimeDigitized,
                new TagDetails(IfdId.Exif,
                               Tag.SubsecTimeDigitized,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               -1)
            },
            {
                TagId.AmbientTemperature,
                new TagDetails(IfdId.Exif,
                               Tag.AmbientTemperature,
                               DataType.SRational,
                               true,
                               false,
                               typeof(SRationalProperty))
                               { DisplayConverter = new AddCUnitConverter() }
            },
            {
                TagId.FlashPixVersion,
                new TagDetails(IfdId.Exif,
                               Tag.FlashPixVersion,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(FourCharProperty),
                               -1)
            },
            {
                TagId.ColorSpace,
                new TagDetails(IfdId.Exif,
                               Tag.ColorSpace,
                               DataType.UShort,
                               true,
                               false,
                               typeof(ColorSpaceProperty))
            },
            {
                TagId.PixelXDimension,
                new TagDetails(IfdId.Exif,
                               Tag.PixelXDimension,
                               new[] { DataType.UShort, DataType.ULong },
                               true,
                               false,
                               typeof(UShortUIntProperty))
            },
            {
                TagId.PixelYDimension,
                new TagDetails(IfdId.Exif,
                               Tag.PixelYDimension,
                               new[] { DataType.UShort, DataType.ULong },
                               true,
                               false,
                               typeof(UShortUIntProperty))
            },
            {
                TagId.RelatedSoundFile,
                new TagDetails(IfdId.Exif,
                               Tag.RelatedSoundFile,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               -1)
            },
            {
                TagId.InteropIfdPointer,
                new TagDetails(IfdId.Exif,
                               Tag.InteropIfdPointer,
                               DataType.ULong,
                               false,
                               true,
                               typeof(UIntProperty))
            },
            {
                TagId.FlashEnergy,
                new TagDetails(IfdId.Exif,
                               Tag.FlashEnergy,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty))
            },
            {
                TagId.SpatialFrequencyResponse,
                new TagDetails(IfdId.Exif,
                               Tag.SpatialFrequencyResponse,
                               DataType.Undefined,
                               false,
                               false,
                               typeof(BinaryProperty),
                               -1)
            },
            {
                TagId.FocalPlaneXResolution,
                new TagDetails(IfdId.Exif,
                               Tag.FocalPlaneXResolution,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty))
            },
            {
                TagId.FocalPlaneYResolution,
                new TagDetails(IfdId.Exif,
                               Tag.FocalPlaneYResolution,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty))
            },
            {
                TagId.FocalPlaneResolutionUnit,
                new TagDetails(IfdId.Exif,
                               Tag.FocalPlaneResolutionUnit,
                               DataType.UShort,
                               true,
                               false,
                               typeof(FocalPlaneResolutionUnitProperty))
            },
            {
                TagId.SubjectLocation,
                new TagDetails(IfdId.Exif,
                               Tag.SubjectLocation,
                               DataType.UShort,
                               true,
                               false,
                               typeof(UShortProperty),
                               2)
            },
            {
                TagId.SensingMethod,
                new TagDetails(IfdId.Exif,
                               Tag.SensingMethod,
                               DataType.UShort,
                               true,
                               false,
                               typeof(UShortProperty))
            },
            {
                TagId.FileSource,
                new TagDetails(IfdId.Exif,
                               Tag.FileSource,
                               DataType.Undefined,
                               true,
                               false,
                               typeof(BinaryProperty),
                               -1)
            },
            {
                TagId.SceneType,
                new TagDetails(IfdId.Exif,
                               Tag.SceneType,
                               DataType.Undefined,
                               true,
                               false,
                               typeof(BinaryProperty),
                               -1)
            },
            {
                TagId.CfaPattern,
                new TagDetails(IfdId.Exif,
                               Tag.CfaPattern,
                               DataType.Undefined,
                               true,
                               false,
                               typeof(BinaryProperty),
                               -1)
            },
            {
                TagId.CustomRendered,
                new TagDetails(IfdId.Exif,
                               Tag.CustomRendered,
                               DataType.UShort,
                               true,
                               false,
                               typeof(CustomRenderedProperty))
            },
            {
                TagId.ExposureMode,
                new TagDetails(IfdId.Exif,
                               Tag.ExposureMode,
                               DataType.UShort,
                               true,
                               false,
                               typeof(ExposureModeProperty))
            },
            {
                TagId.WhiteBalance,
                new TagDetails(IfdId.Exif,
                               Tag.WhiteBalance,
                               DataType.UShort,
                               true,
                               false,
                               typeof(WhiteBalanceProperty))
            },
            {
                TagId.DigitalZoomRatio,
                new TagDetails(IfdId.Exif,
                               Tag.DigitalZoomRatio,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty))
            },
            {
                TagId.FocalLengthIn35mmFilm,
                new TagDetails(IfdId.Exif,
                               Tag.FocalLengthIn35mmFilm,
                               DataType.UShort,
                               true,
                               false,
                               typeof(UShortProperty))
            },
            {
                TagId.SceneCaptureType,
                new TagDetails(IfdId.Exif,
                               Tag.SceneCaptureType,
                               DataType.UShort,
                               true,
                               false,
                               typeof(SceneCaptureTypeProperty))
            },
            {
                TagId.GainControl,
                new TagDetails(IfdId.Exif,
                               Tag.GainControl,
                               DataType.UShort,
                               true,
                               false,
                               typeof(UShortProperty))
            },
            {
                TagId.Contrast,
                new TagDetails(IfdId.Exif,
                               Tag.Contrast,
                               DataType.UShort,
                               true,
                               false,
                               typeof(UShortProperty))
            },
            {
                TagId.Saturation,
                new TagDetails(IfdId.Exif,
                               Tag.Saturation,
                               DataType.UShort,
                               true,
                               false,
                               typeof(UShortProperty))
            },
            {
                TagId.Sharpness,
                new TagDetails(IfdId.Exif,
                               Tag.Sharpness,
                               DataType.UShort,
                               true,
                               false,
                               typeof(UShortProperty))
            },
            {
                TagId.DeviceSettingDescription,
                new TagDetails(IfdId.Exif,
                               Tag.DeviceSettingDescription,
                               DataType.Undefined,
                               false,
                               false,
                               typeof(BinaryProperty),
                               -1)
            },
            {
                TagId.SubjectDistanceRange,
                new TagDetails(IfdId.Exif,
                               Tag.SubjectDistanceRange,
                               DataType.UShort,
                               true,
                               false,
                               typeof(UShortProperty))
            },
            {
                TagId.ImageUniqueId,
                new TagDetails(IfdId.Exif,
                               Tag.ImageUniqueId,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               -1)
            },
            {
                TagId.CameraOwnerName,
                new TagDetails(IfdId.Exif,
                               Tag.CameraOwnerName,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               -1)
            },
            {
                TagId.BodySerialNumber,
                new TagDetails(IfdId.Exif,
                               Tag.BodySerialNumber,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               -1)
            },
            {
                TagId.LensSpecification,
                new TagDetails(IfdId.Exif,
                               Tag.LensSpecification,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty),
                               4)
            },
            {
                TagId.LensMake,
                new TagDetails(IfdId.Exif,
                               Tag.LensMake,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               -1)
            },
            {
                TagId.LensModel,
                new TagDetails(IfdId.Exif,
                               Tag.LensModel,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               -1)
            },
            {
                TagId.LensSerialNumber,
                new TagDetails(IfdId.Exif,
                               Tag.LensSerialNumber,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               -1)
            },
            {
                TagId.Padding,
                new TagDetails(IfdId.Exif,
                               Tag.Padding,
                               DataType.Undefined,
                               true,
                               true,
                               typeof(BinaryProperty),
                               -1)
            },
            {
                TagId.OffsetSchema,
                new TagDetails(IfdId.Exif,
                               Tag.OffsetSchema,
                               DataType.SLong,
                               true,
                               false,
                               typeof(SIntProperty))
            },
            {
                TagId.GpsVersionId,
                new TagDetails(IfdId.Gps,
                               Tag.GpsVersionId,
                               DataType.Byte,
                               true,
                               false,
                               typeof(ByteProperty),
                               4)
            },
            {
                TagId.GpsLatitudeRef,
                new TagDetails(IfdId.Gps,
                               Tag.GpsLatitudeRef,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty),
                               3)
            },
            {
                TagId.GpsLatitude,
                new TagDetails(IfdId.Gps,
                               Tag.GpsLatitude,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               2)
            },
            {
                TagId.GpsLongitudeRef,
                new TagDetails(IfdId.Gps,
                               Tag.GpsLongitudeRef,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               2)
            },
            {
                TagId.GpsLongitude,
                new TagDetails(IfdId.Gps,
                               Tag.GpsLongitude,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty),
                               3)
            },
            {
                TagId.GpsAltitudeRef,
                new TagDetails(IfdId.Gps,
                               Tag.GpsAltitudeRef,
                               DataType.Byte,
                               true,
                               false,
                               typeof(ByteProperty),
                               -1)
            },
            {
                TagId.GpsAltitude,
                new TagDetails(IfdId.Gps,
                               Tag.GpsAltitude,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty))
            },
            {
                TagId.GpsTimeStamp,
                new TagDetails(IfdId.Gps,
                               Tag.GpsTimeStamp,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty),
                               3)
            },
            {
                TagId.GpsSatellites,
                new TagDetails(IfdId.Gps,
                               Tag.GpsSatellites,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               -1)
            },
            {
                TagId.GpsStatus,
                new TagDetails(IfdId.Gps,
                               Tag.GpsStatus,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               2)
            },
            {
                TagId.GpsMeasureMode,
                new TagDetails(IfdId.Gps,
                               Tag.GpsMeasureMode,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               2)
            },
            {
                TagId.GpsDop,
                new TagDetails(IfdId.Gps,
                               Tag.GpsDop,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty))
            },
            {
                TagId.GpsSpeedRef,
                new TagDetails(IfdId.Gps,
                               Tag.GpsSpeedRef,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               2)
            },
            {
                TagId.GpsSpeed,
                new TagDetails(IfdId.Gps,
                               Tag.GpsSpeed,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty))
            },
            {
                TagId.GpsTrackRef,
                new TagDetails(IfdId.Gps,
                               Tag.GpsTrackRef,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               2)
            },
            {
                TagId.GpsTrack,
                new TagDetails(IfdId.Gps,
                               Tag.GpsTrack,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty))
            },
            {
                TagId.GpsImgDirectionRef,
                new TagDetails(IfdId.Gps,
                               Tag.GpsImgDirectionRef,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty))
            },
            {
                TagId.GpsImgDirection,
                new TagDetails(IfdId.Gps,
                               Tag.GpsImgDirection,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               2)
            },
            {
                TagId.GpsMapDatum,
                new TagDetails(IfdId.Gps,
                               Tag.GpsMapDatum,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               -1)
            },
            {
                TagId.GpsDestLatitudeRef,
                new TagDetails(IfdId.Gps,
                               Tag.GpsDestLatitudeRef,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               2)
            },
            {
                TagId.GpsDestLatitude,
                new TagDetails(IfdId.Gps,
                               Tag.GpsDestLatitude,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty),
                               3)
            },
            {
                TagId.GpsDestLongitudeRef,
                new TagDetails(IfdId.Gps,
                               Tag.GpsDestLongitudeRef,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               2)
            },
            {
                TagId.GpsDestLongitude,
                new TagDetails(IfdId.Gps,
                               Tag.GpsDestLongitude,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty),
                               3)
            },
            {
                TagId.GpsDestBearingRef,
                new TagDetails(IfdId.Gps,
                               Tag.GpsDestBearingRef,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               2)
            },
            {
                TagId.GpsDestBearing,
                new TagDetails(IfdId.Gps,
                               Tag.GpsDestBearing,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty))
            },
            {
                TagId.GpsDestDistanceRef,
                new TagDetails(IfdId.Gps,
                               Tag.GpsDestDistanceRef,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(StringProperty),
                               2)
            },
            {
                TagId.GpsDestDistance,
                new TagDetails(IfdId.Gps,
                               Tag.GpsDestDistance,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty))
            },
            {
                TagId.GpsProcessingMethod,
                new TagDetails(IfdId.Gps,
                               Tag.GpsProcessingMethod,
                               DataType.Undefined,
                               true,
                               false,
                               typeof(MulticodeAsciiProperty),
                               -1)
            },
            {
                TagId.GpsAreaInformation,
                new TagDetails(IfdId.Gps,
                               Tag.GpsAreaInformation,
                               DataType.Undefined,
                               true,
                               false,
                               typeof(MulticodeAsciiProperty),
                               -1)
            },
            {
                TagId.GpsDateStamp,
                new TagDetails(IfdId.Gps,
                               Tag.GpsDateStamp,
                               DataType.Ascii,
                               true,
                               false,
                               typeof(GpsDateProperty),
                               -1)
            },
            {
                TagId.GpsDifferential,
                new TagDetails(IfdId.Gps,
                               Tag.GpsDifferential,
                               DataType.UShort,
                               true,
                               false,
                               typeof(UShortProperty))
            },
            {
                TagId.GpsHPositioningError,
                new TagDetails(IfdId.Gps,
                               Tag.GpsHPositioningError,
                               DataType.URational,
                               true,
                               false,
                               typeof(URationalProperty))
            },
        };
    }
}
