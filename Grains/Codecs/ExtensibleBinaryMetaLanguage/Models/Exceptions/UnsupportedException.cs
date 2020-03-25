using System;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Exceptions
{
	public class UnsupportedException : Exception
	{
		public UnsupportedException(string message)
			: base(message)
		{
		}
	}
}