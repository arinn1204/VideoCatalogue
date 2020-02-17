using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;

namespace Grains.Helpers
{
    public static class QueryHelpers
    {
        public static string BuildQueryParameters(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            return parameters
                .Aggregate(
                    "?",
                    (acc, current) =>
                    {
                        var key = UrlEncoder.Default.Encode(current.Key);
                        var value = UrlEncoder.Default.Encode(current.Value);

                        acc += $"{key}={value}&";

                        return acc;
                    },
                    result => result.EndsWith('&') ? result.Trim('&') : result);
        }
    }
}
