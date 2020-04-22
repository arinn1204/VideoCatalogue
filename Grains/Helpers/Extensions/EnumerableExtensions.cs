using System;
using System.Collections.Generic;

namespace Grains.Helpers.Extensions
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<TResult> Throw<TResult, TException>(
			this IEnumerable<TResult> @this,
			TException exceptionToThrow)
			where TException : Exception
		{
			using var enumerator = @this.GetEnumerator();

			while (enumerator.MoveNext())
			{
				yield return enumerator.Current;
				throw exceptionToThrow;
			}
		}

		public static async IAsyncEnumerable<TResult> Throw<TResult, TException>(
			this IAsyncEnumerable<TResult> @this,
			TException exceptionToThrow)
			where TException : Exception
		{
			var enumerator = @this.GetAsyncEnumerator();

			while (await enumerator.MoveNextAsync().ConfigureAwait(false))
			{
				yield return enumerator.Current;
				throw exceptionToThrow;
			}
		}
	}
}