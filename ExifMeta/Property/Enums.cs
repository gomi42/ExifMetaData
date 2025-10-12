namespace ExifMeta
{
    public enum SubFileType : uint
    {
        FullResolutionImage = 0x00,
        ReducedResolutionImage = 0x01,
        SinglePageOfMultiPageImage = 0x02,
        SinglePageOfMultiPageReducedResolutionImage = 0x03,
        TransparencyMask = 0x04,
        TransparencyMaskOfReducedResolutionImage = 0x05,
        TransparencyMaskOfMultiPageImage = 0x06,
        TransparencyMaskOfReducedResolutionMultiPageImage = 0x07,
        DepthMap = 0x08,
        DepthMapOfReducedResolutionImage = 0x09,
        EnhancedImageData = 0x10,
        AlternateReducedResolutionImage = 0x10001,
        SemanticMask = 0x10004,
        Invalid = 0xffffffff,
    }

    public enum LengthUnit : ushort
    {
        None = 0,
        Cm = 1,
        Inch = 2
    }

    public enum Orientation : ushort
    {
        Horizontal = 1,
        MirrorHorizontal = 2,
        Rotate180 = 3,
        MirrorVertical = 4,
        MirrorHorizontalAndRotate270CW = 5,
        Rotate90CW = 6,
        MirrorHorizontalAndRotate90CW = 7,
        Rotate270CW = 8
    }

    public enum ColorSpace : ushort
    {
        sRGB = 1,
        AdobeRGB = 2,
        Uncalibrated = 0xffff,
        ICCProfile = 0xfffe,
        WideGamutRGB = 0xfffd
    }

    public enum ExposureMode : ushort
    {
        Auto = 0,
        Manual = 1,
        Autobracket = 2
    }

    public enum ExposureProgram : ushort
    {
        NotDefined = 0,
        Manual = 1,
        ProgramAE = 2,
        AperturePriority = 3,
        ShutterSpeedPriority = 4,
        CreativeSlowSpeed = 5,
        ActionHighSpeed = 6,
        Portrait = 7,
        Landscape = 8,
        Bulb = 9
    }

    public enum PlanarConfiguration : ushort
    {
        Chunky = 1,
        Planar = 2
    }

    public enum Predictor : ushort
    {
        None = 1,
        HorizontalDifferencing = 2,
        FloatingPoint = 3,
        HorizontalDifferenceX2 = 34892,
        HorizontalDifferenceX4 = 34893,
        FloatingPointX2 = 34894,
        FloatingPointX4 = 34895
    }

    public enum Compression : ushort
    {
        Uncompressed = 1,
        CCITT1D = 2,
        LZW = 5,
        JPEG_old_style = 6,
        JPEG = 7,
        AdobeDeflate = 8,
        JPEG2000 = 34712,
        WebP_old = 34927,
        PNG = 34933,
        JPEGXR = 34934,
        WebP = 50001,
        JPEGXL_old = 50002,
        JPEGXL = 52546
    }

    public enum WhiteBalance : ushort
    {
        Auto = 0,
        Manual = 1
    }

    public enum MeteringMode : ushort
    {
        Unknown = 0,
        Average = 1,
        CenterWeightedAverage = 2,
        Spot = 3,
        MultiSpot = 4,
        MultiSegment = 5,
        Partial = 6,
        Other = 255
    }

    public enum FocalPlaneResolutionUnit : ushort
    {
        None = 1,
        inch = 2,
        cm = 3,
        mm = 4,
        um = 5
    }

    public enum SensitivityType : ushort
    {
        Unknown = 0,
        StandardOutputSensitivity = 1,
        RecommendedExposureIndex = 2,
        ISOSpeed = 3,
        StandardOutputSensitivityAndRecommendedExposureIndex = 4,
        StandardOutputSensitivityAndISOSpeed = 5,
        RecommendedExposureIndexAndISOSpeed = 6,
        StandardOutputSensitivityRecommendedExposureIndexAndISOSpeed = 7
    }

    public enum SceneCaptureType : ushort
    {
        Standard = 0,
        Landscape = 1,
        Portrait = 2,
        Night = 3,
        Other = 4
    }

    public enum CustomRendered : ushort
    {
        Normal = 0,
        Custom = 1,
        HDR_NoOriginalSaved = 2,
        HDR_OriginalSaved = 3,
        Original_ForHDR = 4,
        Panorama = 6,
        PortraitHDR = 7,
        Portrait = 8
    }

    public enum PhotometricInterpretation : ushort
    {
        WhiteIsZero = 0,
        BlackIsZero = 1,
        RGB = 2,
        RGBPalette = 3,
        TransparencyMask = 4,
        CMYK = 5,
        YCbCr = 6,
        CIELab = 7,
        ICCLab = 8,
        ITULab = 9,
        ColorFilterArray = 32803,
        PixarLogL = 32844,
        PixarLogLuv = 32845,
        SequentialColorFilter = 32892,
        LinearRaw = 34892,
        DepthMap = 51177,
        SemanticMask = 52527
    }

    public enum Flash : ushort
    {
        NoFlash = 0x0,
        Fired = 0x1,
        FiredReturnNotNetected = 0x5,
        FiredReturnDetected = 0x7,
        OnDidNotFire = 0x8,
        OnFired = 0x9,
        OnReturnNotDetected = 0xd,
        OnReturnDetected = 0xf,
        OffDidNotFire = 0x10,
        OffDidNotFireReturnNotNetected = 0x14,
        AutoDidNotFire = 0x18,
        AutoFired = 0x19,
        AutoFiredReturnNotDetected = 0x1d,
        AutoFiredReturnDetected = 0x1f,
        NoflashFunction = 0x20,
        OffNoFlashFunction = 0x30,
        FiredRedEyeReduction = 0x41,
        FiredRedEyeReductionReturnNotDetected = 0x45,
        FiredRedEyeEeductionReturnDetected = 0x47,
        OnRedEyeReduction = 0x49,
        OnRedEyeReductionReturnNotDetected = 0x4d,
        OnRedEyeReductionReturnDetected = 0x4f,
        OffRedEyeReduction = 0x50,
        AutoDidNotFireRedEyeReduction = 0x58,
        AutoFiredRedEyeReduction = 0x59,
        AutoFiredRedEyeReductionReturnNotDetected = 0x5d,
        AutoFiredRedEyeReductionReturnDetected = 0x5f
    }
}


