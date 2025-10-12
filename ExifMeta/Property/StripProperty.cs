using System.IO;

namespace ExifMeta
{
    internal class StripProperty
    {
        public Stream SourceStream;
        public uint[] SourceOffsets;
        public uint[] Counts;
    }
}
