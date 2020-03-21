using System;

namespace Grains.CodecParser.Matroska
{
    public class MatroskaException : Exception
    {
        public MatroskaException(string message) : base(message)
        {
        }
    }
}