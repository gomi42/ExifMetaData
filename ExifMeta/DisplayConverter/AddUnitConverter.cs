namespace ExifMeta
{
    internal class AddMmUnitConverter : IDisplayConverter
    {
        public string ToString(object value)
        {
            var ur = ((URational)value).ToDecimal();
            return $"{ur.ToString("G1")} mm";
        }
    }

    internal class AddCUnitConverter : IDisplayConverter
    {
        public string ToString(object value)
        {
            var ur = ((SRational)value).ToDecimal();
            return $"{ur.ToString("G1")} °C";
        }
    }
}
