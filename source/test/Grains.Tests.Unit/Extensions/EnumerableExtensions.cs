using System;
using System.Collections.Generic;

namespace Grains.Tests.Unit.Extensions
{
	public static class EnumerableExtensions
	{
		public static async IAsyncEnumerable<TResult> Throw<TResult, TException>(
			this IAsyncEnumerable<TResult> @this,
			TException exceptionToThrow)
			where TException : Exception
		{
			var enumerator = @this.GetAsyncEnumerator();

			while (await enumerator.MoveNextAsync())
			{
				yield return enumerator.Current;
				throw exceptionToThrow;
			}
		}
	}
}