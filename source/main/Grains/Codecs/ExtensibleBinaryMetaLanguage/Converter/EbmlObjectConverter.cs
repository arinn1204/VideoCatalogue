using System;
using System.Reflection;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Converter
{
	public static class EbmlObjectConverter
	{
		public static object? HandleSingleObject(
			PropertyInfo propertyToSet,
			object? value)
		{
			var valueToSet = value;
			var underlyingType = Nullable.GetUnderlyingType(propertyToSet.PropertyType);
			if (underlyingType != null && underlyingType != value?.GetType())
			{
				valueToSet = Convert.ChangeType(value, underlyingType);
			}

			return valueToSet;
		}
	}
}