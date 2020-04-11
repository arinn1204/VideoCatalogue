#nullable enable
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Grains.Codecs;

namespace Grains.Tests.Unit.Extensions
{
	public static class ObjectExtensions
	{
		public static int GetSize(this object @this)
		{
			var count = 0;
			foreach (var property in @this.GetType().GetProperties())
			{
				if (property.PropertyType.Assembly == typeof(Parser).Assembly)
				{
					var value = property.GetValue(@this);
					count += value?.GetSize() + 1 ?? 1;
				}
				else if (property.PropertyType.IsInterface &&
				         property.PropertyType.GetInterfaces().Any(a => a == typeof(IEnumerable)) &&
				         property.PropertyType.GetGenericArguments().First().Assembly ==
				         typeof(Parser).Assembly)
				{
					count = HandleEnumerableOfCustomType(@this, property, count);
				}
				else if (property.PropertyType.IsInterface &&
				         property.PropertyType.GetInterfaces().Any(a => a == typeof(IEnumerable)))
				{
					count = GetCountOfEnumerable(@this, property, count);
				}
				else
				{
					count++;
				}
			}

			return count;
		}

		private static int GetCountOfEnumerable(object @this, PropertyInfo property, int count)
		{
			var underlyingType = property.PropertyType.GetGenericArguments().First();
			var enumerable = property.GetValue(@this) ?? GetEmptyEnumerable(underlyingType);
			var numberInEnumerable =
				enumerable == null
					? 0
					: typeof(Enumerable).GetMethods()
					                    .Single(
						                     w => w.Name == "Count" &&
						                          w.IsGenericMethod &&
						                          w.GetParameters().Length == 1)
					                   ?.MakeGenericMethod(underlyingType)
					                   ?.Invoke(
						                     null,
						                     new[]
						                     {
							                     enumerable
						                     });
			count += numberInEnumerable as int? ?? 0;
			return count;
		}

		private static int HandleEnumerableOfCustomType(
			object @this,
			PropertyInfo property,
			int count)
		{
			var underlyingType = property.PropertyType.GetGenericArguments().First();
			var enumerable = property.GetValue(@this) ?? GetEmptyEnumerable(underlyingType);
			var numberInEnumerable =
				typeof(Enumerable).GetMethods()
				                  .Single(
					                   w => w.Name == "Count" &&
					                        w.IsGenericMethod &&
					                        w.GetParameters().Length == 1)
				                 ?.MakeGenericMethod(underlyingType)
				                 ?.Invoke(
					                   null,
					                   new[]
					                   {
						                   enumerable
					                   });

			var firstOfEnumerable = typeof(Enumerable)
			                       .GetMethods()
			                       .Single(
				                        s => s.Name == "FirstOrDefault" &&
				                             s.IsGenericMethod &&
				                             s.GetParameters().Length == 1)
			                      ?.MakeGenericMethod(underlyingType)
			                      ?.Invoke(
				                        null,
				                        new[]
				                        {
					                        enumerable
				                        });

			count += (int) (numberInEnumerable ?? 0) * (firstOfEnumerable?.GetSize() + 1 ?? 0);
			return count;
		}

		private static object? GetEmptyEnumerable(Type underlyingType) => typeof(Enumerable)
		                                                                 .GetMethod("Empty")
		                                                                ?.MakeGenericMethod(
			                                                                  underlyingType)
		                                                                 .Invoke(null, null);
	}
}