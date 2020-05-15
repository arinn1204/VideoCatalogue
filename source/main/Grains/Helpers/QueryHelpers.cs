using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;

namespace Grains.Helpers
{
	public static class QueryHelpers
	{
		public static string BuildQueryParameters(
			IEnumerable<KeyValuePair<string, string>> parameters)
		{
			return parameters
			   .Aggregate(
					"?",
					(acc, current) =>
					{
						var (key, value) = current;
						var encodedKey = UrlEncoder.Default.Encode(key);
						var encodedValue = UrlEncoder.Default.Encode(value);

						acc += $"{encodedKey}={encodedValue}&";

						return acc;
					},
					result => result.EndsWith('&')
						? result.Trim('&')
						: result);
		}
	}
}