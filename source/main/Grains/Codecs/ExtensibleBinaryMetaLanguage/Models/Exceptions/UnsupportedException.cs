using System;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Exceptions
{
	public class UnsupportedException : Exception
	{
		public UnsupportedException(string parameterName, string unsupportedValue)
			: base(
				$"'{parameterName}' with a value of '{unsupportedValue}' is not supported at this time.")
		{
		}
	}
}