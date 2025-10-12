// Automatically generated file. Do not modify.

using System;

namespace ExifMeta
{
    public partial class Ifd
    {
        public byte GetByteProperty(TagId tagId)
        {
            return ((ByteProperty)GetProperty(tagId)).Values[0];
        }

        public byte[] GetBytesProperty(TagId tagId)
        {
            return ((ByteProperty)GetProperty(tagId)).Values;
        }

        public void SetByteProperty(TagId tagId, byte value)
        {
            SetProperty(new ByteProperty(tagId, value));
        }

        public void SetBytesProperty(TagId tagId, byte[] values)
        {
            SetProperty(new ByteProperty(tagId, values));
        }

        public sbyte GetSByteProperty(TagId tagId)
        {
            return ((SByteProperty)GetProperty(tagId)).Values[0];
        }

        public sbyte[] GetSBytesProperty(TagId tagId)
        {
            return ((SByteProperty)GetProperty(tagId)).Values;
        }

        public void SetSByteProperty(TagId tagId, sbyte value)
        {
            SetProperty(new SByteProperty(tagId, value));
        }

        public void SetSBytesProperty(TagId tagId, sbyte[] values)
        {
            SetProperty(new SByteProperty(tagId, values));
        }

        public ushort GetUShortProperty(TagId tagId)
        {
            return ((UShortProperty)GetProperty(tagId)).Values[0];
        }

        public ushort[] GetUShortsProperty(TagId tagId)
        {
            return ((UShortProperty)GetProperty(tagId)).Values;
        }

        public void SetUShortProperty(TagId tagId, ushort value)
        {
            SetProperty(new UShortProperty(tagId, value));
        }

        public void SetUShortsProperty(TagId tagId, ushort[] values)
        {
            SetProperty(new UShortProperty(tagId, values));
        }

        public short GetSShortProperty(TagId tagId)
        {
            return ((SShortProperty)GetProperty(tagId)).Values[0];
        }

        public short[] GetSShortsProperty(TagId tagId)
        {
            return ((SShortProperty)GetProperty(tagId)).Values;
        }

        public void SetSShortProperty(TagId tagId, short value)
        {
            SetProperty(new SShortProperty(tagId, value));
        }

        public void SetSShortsProperty(TagId tagId, short[] values)
        {
            SetProperty(new SShortProperty(tagId, values));
        }

        public int GetSIntProperty(TagId tagId)
        {
            return ((SIntProperty)GetProperty(tagId)).Values[0];
        }

        public int[] GetSIntsProperty(TagId tagId)
        {
            return ((SIntProperty)GetProperty(tagId)).Values;
        }

        public void SetSIntProperty(TagId tagId, int value)
        {
            SetProperty(new SIntProperty(tagId, value));
        }

        public void SetSIntsProperty(TagId tagId, int[] values)
        {
            SetProperty(new SIntProperty(tagId, values));
        }

        public URational GetURationalProperty(TagId tagId)
        {
            return ((URationalProperty)GetProperty(tagId)).Values[0];
        }

        public URational[] GetURationalsProperty(TagId tagId)
        {
            return ((URationalProperty)GetProperty(tagId)).Values;
        }

        public void SetURationalProperty(TagId tagId, URational value)
        {
            SetProperty(new URationalProperty(tagId, value));
        }

        public void SetURationalsProperty(TagId tagId, URational[] values)
        {
            SetProperty(new URationalProperty(tagId, values));
        }

        public SRational GetSRationalProperty(TagId tagId)
        {
            return ((SRationalProperty)GetProperty(tagId)).Values[0];
        }

        public SRational[] GetSRationalsProperty(TagId tagId)
        {
            return ((SRationalProperty)GetProperty(tagId)).Values;
        }

        public void SetSRationalProperty(TagId tagId, SRational value)
        {
            SetProperty(new SRationalProperty(tagId, value));
        }

        public void SetSRationalsProperty(TagId tagId, SRational[] values)
        {
            SetProperty(new SRationalProperty(tagId, values));
        }

        public float GetFloatProperty(TagId tagId)
        {
            return ((FloatProperty)GetProperty(tagId)).Values[0];
        }

        public void SetFloatProperty(TagId tagId, float value)
        {
            SetProperty(new FloatProperty(tagId, value));
        }

        public double GetDoubleProperty(TagId tagId)
        {
            return ((DoubleProperty)GetProperty(tagId)).Values[0];
        }

        public void SetDoubleProperty(TagId tagId, double value)
        {
            SetProperty(new DoubleProperty(tagId, value));
        }

        public DateTime GetDateTimeProperty(TagId tagId)
        {
            return ((DateTimeProperty)GetProperty(tagId)).Value;
        }

        public void SetDateTimeProperty(TagId tagId, DateTime value)
        {
            SetProperty(new DateTimeProperty(tagId, value));
        }

        public DateTime GetGpsDateProperty(TagId tagId)
        {
            return ((GpsDateProperty)GetProperty(tagId)).Value;
        }

        public void SetGpsDateProperty(TagId tagId, DateTime value)
        {
            SetProperty(new GpsDateProperty(tagId, value));
        }

        public TimeSpan GetTimeOffsetProperty(TagId tagId)
        {
            return ((TimeOffsetProperty)GetProperty(tagId)).Value;
        }

        public void SetTimeOffsetProperty(TagId tagId, TimeSpan value)
        {
            SetProperty(new TimeOffsetProperty(tagId, value));
        }

        public double GetApex1Property(TagId tagId)
        {
            return ((Apex1Property)GetProperty(tagId)).Value;
        }

        public void SetApex1Property(TagId tagId, double value)
        {
            SetProperty(new Apex1Property(tagId, value));
        }

        public double GetApex2Property(TagId tagId)
        {
            return ((Apex2Property)GetProperty(tagId)).Value;
        }

        public void SetApex2Property(TagId tagId, double value)
        {
            SetProperty(new Apex2Property(tagId, value));
        }

        public SubFileType GetSubFileTypeProperty(TagId tagId)
        {
            return ((SubFileTypeProperty)GetProperty(tagId)).Value;
        }

        public void SetSubFileTypeProperty(TagId tagId, SubFileType value)
        {
            SetProperty(new SubFileTypeProperty(tagId, value));
        }

        public LengthUnit GetLengthUnitProperty(TagId tagId)
        {
            return ((LengthUnitProperty)GetProperty(tagId)).Value;
        }

        public void SetLengthUnitProperty(TagId tagId, LengthUnit value)
        {
            SetProperty(new LengthUnitProperty(tagId, value));
        }

        public Orientation GetOrientationProperty(TagId tagId)
        {
            return ((OrientationProperty)GetProperty(tagId)).Value;
        }

        public void SetOrientationProperty(TagId tagId, Orientation value)
        {
            SetProperty(new OrientationProperty(tagId, value));
        }

        public ColorSpace GetColorSpaceProperty(TagId tagId)
        {
            return ((ColorSpaceProperty)GetProperty(tagId)).Value;
        }

        public void SetColorSpaceProperty(TagId tagId, ColorSpace value)
        {
            SetProperty(new ColorSpaceProperty(tagId, value));
        }

        public ExposureMode GetExposureModeProperty(TagId tagId)
        {
            return ((ExposureModeProperty)GetProperty(tagId)).Value;
        }

        public void SetExposureModeProperty(TagId tagId, ExposureMode value)
        {
            SetProperty(new ExposureModeProperty(tagId, value));
        }

        public ExposureProgram GetExposureProgramProperty(TagId tagId)
        {
            return ((ExposureProgramProperty)GetProperty(tagId)).Value;
        }

        public void SetExposureProgramProperty(TagId tagId, ExposureProgram value)
        {
            SetProperty(new ExposureProgramProperty(tagId, value));
        }

        public PlanarConfiguration GetPlanarConfigurationProperty(TagId tagId)
        {
            return ((PlanarConfigurationProperty)GetProperty(tagId)).Value;
        }

        public void SetPlanarConfigurationProperty(TagId tagId, PlanarConfiguration value)
        {
            SetProperty(new PlanarConfigurationProperty(tagId, value));
        }

        public Predictor GetPredictorProperty(TagId tagId)
        {
            return ((PredictorProperty)GetProperty(tagId)).Value;
        }

        public void SetPredictorProperty(TagId tagId, Predictor value)
        {
            SetProperty(new PredictorProperty(tagId, value));
        }

        public Compression GetCompressionProperty(TagId tagId)
        {
            return ((CompressionProperty)GetProperty(tagId)).Value;
        }

        public void SetCompressionProperty(TagId tagId, Compression value)
        {
            SetProperty(new CompressionProperty(tagId, value));
        }

        public WhiteBalance GetWhiteBalanceProperty(TagId tagId)
        {
            return ((WhiteBalanceProperty)GetProperty(tagId)).Value;
        }

        public void SetWhiteBalanceProperty(TagId tagId, WhiteBalance value)
        {
            SetProperty(new WhiteBalanceProperty(tagId, value));
        }

        public MeteringMode GetMeteringModeProperty(TagId tagId)
        {
            return ((MeteringModeProperty)GetProperty(tagId)).Value;
        }

        public void SetMeteringModeProperty(TagId tagId, MeteringMode value)
        {
            SetProperty(new MeteringModeProperty(tagId, value));
        }

        public FocalPlaneResolutionUnit GetFocalPlaneResolutionUnitProperty(TagId tagId)
        {
            return ((FocalPlaneResolutionUnitProperty)GetProperty(tagId)).Value;
        }

        public void SetFocalPlaneResolutionUnitProperty(TagId tagId, FocalPlaneResolutionUnit value)
        {
            SetProperty(new FocalPlaneResolutionUnitProperty(tagId, value));
        }

        public SensitivityType GetSensitivityTypeProperty(TagId tagId)
        {
            return ((SensitivityTypeProperty)GetProperty(tagId)).Value;
        }

        public void SetSensitivityTypeProperty(TagId tagId, SensitivityType value)
        {
            SetProperty(new SensitivityTypeProperty(tagId, value));
        }

        public SceneCaptureType GetSceneCaptureTypeProperty(TagId tagId)
        {
            return ((SceneCaptureTypeProperty)GetProperty(tagId)).Value;
        }

        public void SetSceneCaptureTypeProperty(TagId tagId, SceneCaptureType value)
        {
            SetProperty(new SceneCaptureTypeProperty(tagId, value));
        }

        public CustomRendered GetCustomRenderedProperty(TagId tagId)
        {
            return ((CustomRenderedProperty)GetProperty(tagId)).Value;
        }

        public void SetCustomRenderedProperty(TagId tagId, CustomRendered value)
        {
            SetProperty(new CustomRenderedProperty(tagId, value));
        }

        public PhotometricInterpretation GetPhotometricInterpretationProperty(TagId tagId)
        {
            return ((PhotometricInterpretationProperty)GetProperty(tagId)).Value;
        }

        public void SetPhotometricInterpretationProperty(TagId tagId, PhotometricInterpretation value)
        {
            SetProperty(new PhotometricInterpretationProperty(tagId, value));
        }

        public Flash GetFlashProperty(TagId tagId)
        {
            return ((FlashProperty)GetProperty(tagId)).Value;
        }

        public void SetFlashProperty(TagId tagId, Flash value)
        {
            SetProperty(new FlashProperty(tagId, value));
        }

    }
}
