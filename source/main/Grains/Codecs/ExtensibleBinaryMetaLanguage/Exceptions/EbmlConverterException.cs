using System;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Exceptions
{
	public class EbmlConverterException : Exception
	{
		public EbmlConverterException(string message)
			: base(message)
		{
		}
	}
}