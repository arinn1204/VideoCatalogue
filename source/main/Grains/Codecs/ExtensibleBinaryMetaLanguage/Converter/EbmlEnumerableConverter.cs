using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Converter
{
	public static class EbmlEnumerableConverter
	{
		public static object? HandleMultipleObjects(
			PropertyInfo propertyToSet,
			IEnumerable<object?> values)
			=> propertyToSet.PropertyType.IsInterface
				? HandleInterface(propertyToSet, values)
				: HandleConcrete(propertyToSet, values);

		private static object? HandleConcrete(
			PropertyInfo propertyToSet,
			IEnumerable<object?> values)
		{
			var isGenericType = propertyToSet.PropertyType.IsGenericType;

			var conversionType = isGenericType
				? propertyToSet.PropertyType.GenericTypeArguments.First()
				: Type.GetType(
					propertyToSet.PropertyType.UnderlyingSystemType.FullName!.Replace(
						"[]",
						string.Empty))!;

			var enumerableOfUnderlying =
				typeof(IEnumerable<>).MakeGenericType(conversionType);

			return ConvertToConcreteType(
				propertyToSet,
				enumerableOfUnderlying,
				conversionType,
				values);
		}

		private static object? ConvertToConcreteType(
			PropertyInfo propertyInfo,
			Type enumerableType,
			Type underlyingType,
			IEnumerable<object?> values)
		{
			var castedValues = values?.Cast(underlyingType);
			return propertyInfo.PropertyType.IsArray
				? CreateArray(underlyingType, castedValues)
				: propertyInfo.PropertyType
				              .GetConstructor(
					               new[]
					               {
						               enumerableType
					               })
				             ?.Invoke(
					               new[]
					               {
						               castedValues
					               });
		}

		private static object? CreateArray(Type underlyingType, object? data)
		{
			var array = typeof(Enumerable)
			           .GetMethod(nameof(Enumerable.ToArray))
			          ?.MakeGenericMethod(underlyingType)
			          ?.Invoke(
				            null,
				            new[]
				            {
					            data
				            });
			return array;
		}

		private static object? HandleInterface(
			PropertyInfo propertyToSet,
			IEnumerable<object?> values)
			=> HandleIEnumerable(propertyToSet, values);

		private static object? HandleIEnumerable(
			PropertyInfo propertyToSet,
			IEnumerable<object?> values)
		{
			var underlyingTypeForProperty = propertyToSet
			                               .PropertyType
			                               .GetGenericArguments();

			var conversionType = underlyingTypeForProperty[0];
			var returnEnumerable = values.Cast(conversionType);
			var returnObject = returnEnumerable;
			return returnObject;
		}


		private static object? Cast(this IEnumerable<object?> @this, Type typeToCastTo)
		{
			var sourceArray = @this
			                 .Where(w => w != null)
			                 .ToArray();
			var castValue = sourceArray.Length == 1 &&
			                sourceArray.All(a => a!.GetType().IsArray)
				? sourceArray[0]
				: @this;
			return typeof(Enumerable)
			      .GetMethod(nameof(Enumerable.Cast))
			     ?.MakeGenericMethod(typeToCastTo)
			      .Invoke(
				       null,
				       new[]
				       {
					       castValue
				       });
		}
	}
}