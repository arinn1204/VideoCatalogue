using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Grains.Helpers;
using Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces.DetailRepository;
using GrainsInterfaces.Models.VideoApi.Enums;

namespace Grains.VideoInformation.TheMovieDatabaseRepositories
{
	internal class TheMovieDatabaseSearchRepository : ITheMovieDatabaseSearchDetailRepository
	{
#region ITheMovieDatabaseSearchDetailRepository Members

		public Task<HttpResponseMessage> Search(
			string title,
			int? year,
			string version,
			HttpClient client,
			MovieType type)
		{
			using var request = new HttpRequestMessage
			                    {
				                    Method = HttpMethod.Get,
				                    RequestUri = BuildSearchUri(
					                    title,
					                    year,
					                    version,
					                    type)
			                    };

			return client.SendAsync(request);
		}

#endregion

		private Uri BuildSearchUri(
			string title,
			int? year,
			string version,
			MovieType type)
		{
			var parameters =
				Enumerable.Empty<KeyValuePair<string, string>>()
				          .Append(new KeyValuePair<string, string>("query", title))
				          .Append(new KeyValuePair<string, string>("language", "en-US"))
				          .Append(new KeyValuePair<string, string>("include_adult", "true"));

			var videoType = type switch
			                {
				                MovieType.Movie    => "movie",
				                MovieType.TvSeries => "tv",
				                _ => throw new ArgumentException(
					                $"{type} is an unsupported enum type.")
			                };

			if (year.HasValue)
			{
				parameters = parameters.Append(
					new KeyValuePair<string, string>("year", year.Value.ToString()));
			}

			var queryParameters = QueryHelpers.BuildQueryParameters(parameters);

			return new Uri($"{version}/search/{videoType}{queryParameters}", UriKind.Relative);
		}
	}
}