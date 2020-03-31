#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Exceptions;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Converter
{
	public static class EbmlConvert
	{
		public static TTarget? DeserializeTo<TTarget>(params (string name, object value)[] values)
			where TTarget : class, new()
		{
			return typeof(TTarget).CustomAttributes.All(
				a => a.AttributeType != typeof(EbmlMasterAttribute))
				? default
				: CreateTarget<TTarget>(values);
		}

		public static object? DeserializeTo(
			string targetObjectName,
			params (string name, object value)[] values)
		{
			var targetType = typeof(EbmlConvert)
			                .Assembly
			                .DefinedTypes
			                .Where(
				                 w => w.CustomAttributes.Any(
					                 a => a.AttributeType == typeof(EbmlMasterAttribute)))
			                .FirstOrDefault(f => f.Name == targetObjectName);

			return typeof(EbmlConvert)
			      .GetMethod(
				       "DeserializeTo",
				       new[]
				       {
					       typeof((string name, object value)[])
				       })
			     ?.MakeGenericMethod(targetType ?? typeof(object))
			      .Invoke(
				       null,
				       new object?[]
				       {
					       values
				       });
		}

		private static TTarget CreateTarget<TTarget>(
			IEnumerable<(string name, object value)> values)
			where TTarget : new()
		{
			var target = new TTarget();
			foreach (var (name, value) in values)
			{
				var valueToSet = value;
				var propertyByName = typeof(TTarget).GetProperty(name);
				var propertyByAttribute = GetPropertyByAttribute<TTarget>(name);

				var propertyToSet = DeterminePropertyToSet(
					propertyByAttribute,
					propertyByName,
					name);

				var underlyingType = Nullable.GetUnderlyingType(propertyToSet.PropertyType);
				if (underlyingType != null && underlyingType != value.GetType())
				{
					valueToSet = Convert.ChangeType(value, underlyingType);
				}
				else if (underlyingType == null && propertyToSet.PropertyType != value?.GetType())
				{
					valueToSet = Convert.ChangeType(value, propertyToSet.PropertyType);
				}

				propertyToSet?.SetValue(target, valueToSet);
			}

			return target;
		}

		private static PropertyInfo DeterminePropertyToSet(
			PropertyInfo propertyByAttribute,
			PropertyInfo propertyByName,
			string name)
		{
			var propertyToSet = (propertyByAttribute == null, propertyByName == null,
			                     propertyByAttribute?.Name == propertyByName?.Name) switch
			                    {
				                    (true, false, _)     => propertyByName,
				                    (false, true, _)     => propertyByAttribute,
				                    (false, false, true) => propertyByName,
				                    (false, false, false) => throw new EbmlConverterException(
					                    $"Ambiguous match. Element name of '{name}' associated with '{propertyByAttribute?.Name}' and property name '{name}'."),
				                    (true, true, _) => throw new EbmlConverterException(
					                    $"There is no element with the name '{name}'.")
			                    };
			return propertyToSet;
		}

		private static PropertyInfo GetPropertyByAttribute<TTarget>(string name)
			where TTarget : new()
		{
			var propertyByAttribute =
				typeof(TTarget).GetProperties()
				               .Where(
					                w => w.CustomAttributes.Any(
						                a => a.AttributeType ==
						                     typeof(EbmlElementAttribute) &&
						                     (string) a
						                             .ConstructorArguments
						                             .FirstOrDefault()
						                             .Value ==
						                     name))
				               .ToList();

			var propertyToReturn = propertyByAttribute.Count <= 1
				? propertyByAttribute.FirstOrDefault()
				: throw new EbmlConverterException(
					$"Ambiguous match. There are multiple elements with name '{name}'.");

			return propertyToReturn;
		}
	}
}