using System;

namespace Grains.CodecParser.Matroska.Models
{
	public class MatroskaException : Exception
	{
		public MatroskaException(string message)
			: base(message)
		{
		}
	}
}