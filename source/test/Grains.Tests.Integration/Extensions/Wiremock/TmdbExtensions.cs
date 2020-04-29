using System.IO;
using System.Linq;
using GrainsInterfaces.Models.VideoApi;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Grains.Tests.Integration.Extensions.Wiremock
{
	public static class TmdbExtensions
	{
		public static WireMockServer AddMovieCredit(
			this WireMockServer server,
			string title,
			int movieId)
		{
			server.Given(
				       Request.Create()
				              .UsingGet()
				              .WithPath($"/3/movie/{movieId}/credits"))
			      .RespondWith(
				       Response.Create()
				               .WithBody(
					                GetFileData(
						                $"{title.ToFileBaseName().ToFilePath()}.credits.json")));

			return server;
		}

		public static WireMockServer AddMovieDetail(
			this WireMockServer server,
			string title,
			int movieId)
		{
			server.Given(
				       Request.Create()
				              .UsingGet()
				              .WithPath($"/3/movie/{movieId}"))
			      .RespondWith(
				       Response.Create()
				               .WithBody(
					                GetFileData($"{title.ToFileBaseName().ToFilePath()}.json")));

			return server;
		}

		public static WireMockServer AddSearch(
			this WireMockServer server,
			VideoRequest request)
		{
			server.Given(
				       Request.Create()
				              .UsingGet()
				              .WithPath("/3/search/movie")
				              .WithParam(
					               queryParams =>
					               {
						               bool ContainsProperValue(string key, string value)
							               => queryParams.ContainsKey(key) &&
							                  queryParams[key].Single() == value;

						               var containsLanguage = ContainsProperValue(
							               "language",
							               "en-US");
						               var includeAdult = ContainsProperValue(
							               "include_adult",
							               "true");
						               var year = request.Year.HasValue &&
						                          ContainsProperValue(
							                          "year",
							                          request.Year.Value.ToString());

						               var containQuery = ContainsProperValue(
							               "query",
							               request.Title);


						               return containsLanguage &&
						                      includeAdult &&
						                      containQuery &&
						                      (!request.Year.HasValue || year);
					               }))
			      .RespondWith(
				       Response.Create()
				               .WithBody(
					                GetFileData(
						                $"{request.Title.ToFileBaseName().ToFilePath()}.searchResults.json")));

			return server;
		}

		private static string GetFileData(string fileName)
		{
			var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			using var reader = new StreamReader(stream);
			return reader.ReadToEnd();
		}
	}
}