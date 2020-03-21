﻿using System.Net.Http;
using System.Threading.Tasks;

namespace Grains.VideoApi.Interfaces.Repositories.Details
{
	internal interface ITheMovieDatabaseTvEpisodeDetailRepository
	{
		Task<HttpResponseMessage> GetTvSeriesDetail(
			int tvId,
			string baseUrl,
			HttpClient client);

		Task<HttpResponseMessage> GetTvEpisodeDetail(
			int tvId,
			int seasonNumber,
			int episodeNumber,
			string baseUrl,
			HttpClient client);

		Task<HttpResponseMessage> GetTvEpisodeCredits(
			int tvId,
			int seasonNumber,
			int episodeNumber,
			string baseUrl,
			HttpClient client);
	}
}