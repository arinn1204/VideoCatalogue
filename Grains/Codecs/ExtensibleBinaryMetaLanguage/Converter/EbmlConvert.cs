#nullable enable
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

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
				: EbmlTargetConverter.CreateTarget<TTarget>(values);
		}

		public static object? DeserializeTo(
			string targetObjectName,
			params (string name, object value)[] values)
		{
			var masterTypes = typeof(EbmlConvert)
			                 .Assembly
			                 .DefinedTypes
			                 .Where(
				                  w => w.CustomAttributes.Any(
					                  a => a.AttributeType == typeof(EbmlMasterAttribute)));
			var targetType = masterTypes
			   .FirstOrDefault(
					f => f.Name == targetObjectName ||
					     f.CustomAttributes.Any(
						     a => Equals(
							     targetObjectName,
							     a.ConstructorArguments.FirstOrDefault().Value)));

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
	}
}