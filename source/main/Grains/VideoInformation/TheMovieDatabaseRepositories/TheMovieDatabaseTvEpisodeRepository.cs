﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces.DetailRepository;

namespace Grains.VideoInformation.TheMovieDatabaseRepositories
{
	public class TheMovieDatabaseTvEpisodeRepository : ITheMovieDatabaseTvEpisodeDetailRepository
	{
#region ITheMovieDatabaseTvEpisodeDetailRepository Members

		public Task<HttpResponseMessage> GetTvEpisodeCredits(
			int tvId,
			int seasonNumber,
			int episodeNumber,
			string version,
			HttpClient client)
		{
			var tvUrl = GetTvUrl(version, tvId);
			var episodeUrl = GetEpisodeUrl(tvUrl, seasonNumber, episodeNumber);
			using var request = new HttpRequestMessage(
				HttpMethod.Get,
				new Uri($"{episodeUrl.Trim('/')}/credits", UriKind.Relative));

			return client.SendAsync(request);
		}

		public Task<HttpResponseMessage> GetTvEpisodeDetail(
			int tvId,
			int seasonNumber,
			int episodeNumber,
			string version,
			HttpClient client)
		{
			var tvUrl = GetTvUrl(version, tvId);
			var episodeUrl = GetEpisodeUrl(tvUrl, seasonNumber, episodeNumber);
			using var request = new HttpRequestMessage(
				HttpMethod.Get,
				new Uri(episodeUrl, UriKind.Relative));

			return client.SendAsync(request);
		}

		public Task<HttpResponseMessage> GetTvSeriesDetail(
			int tvId,
			string version,
			HttpClient client)
		{
			using var request = new HttpRequestMessage(
				HttpMethod.Get,
				new Uri(GetTvUrl(version, tvId), UriKind.Relative));

			return client.SendAsync(request);
		}

#endregion

		private string GetEpisodeUrl(string tvUrl, int seasonNumber, int episodeNumber)
			=> $"{tvUrl.Trim('/')}/season/{seasonNumber}/episode/{episodeNumber}";

		private string GetTvUrl(string baseUrl, int tvId) => $"{baseUrl.Trim('/')}/tv/{tvId}";
	}
}