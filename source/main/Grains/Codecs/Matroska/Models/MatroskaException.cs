using System;

namespace Grains.Codecs.Matroska.Models
{
	public class MatroskaException : Exception
	{
		public MatroskaException(string message)
			: base(message)
		{
		}
	}
}