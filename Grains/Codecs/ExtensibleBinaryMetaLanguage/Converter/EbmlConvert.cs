using System.Collections.Generic;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Exceptions;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Converter
{
	public static class EbmlConvert
	{
		public static TTarget DeserializeTo<TTarget>(params (string name, object value)[] values)
			where TTarget : new()
		{
			return typeof(TTarget).CustomAttributes.All(
				a => a.AttributeType != typeof(EbmlMasterAttribute))
				? default
				: CreateTarget<TTarget>(values);
		}

		private static TTarget CreateTarget<TTarget>(
			IEnumerable<(string name, object value)> values)
			where TTarget : new()
		{
			var target = new TTarget();
			foreach (var (name, value) in values)
			{
				var propertyByName = typeof(TTarget).GetProperty(name);
				var propertyByAttribute =
					typeof(TTarget).GetProperties()
					               .FirstOrDefault(
						                w => w.CustomAttributes.Any(
							                a => a.AttributeType ==
							                     typeof(EbmlElementAttribute) &&
							                     (string) a
							                             .ConstructorArguments
							                             .FirstOrDefault()
							                             .Value ==
							                     name));

				var propertyToSet = (propertyByAttribute == null, propertyByName == null) switch
				                    {
					                    (true, false) => propertyByName,
					                    (false, true) => propertyByAttribute,
					                    (false, false) => throw new EbmlConverterException(
						                    $"Ambiguous match. Element name of '{name}' associated with '{propertyByAttribute.Name}' and property name '{name}'."),
					                    (true, true) => throw new EbmlConverterException(
						                    $"There is no element with the name '{name}'.")
				                    };

				propertyToSet?.SetValue(target, value);
			}

			return target;
		}
	}
}