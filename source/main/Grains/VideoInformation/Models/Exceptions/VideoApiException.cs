using System;
using System.Collections.Generic;
using Grains.VideoInformation.Models.SearchResults;
using GrainsInterfaces.Models.VideoApi;

namespace Grains.VideoInformation.Models.Exceptions
{
	public class VideoApiException : Exception
	{
		public VideoApiException(string message, VideoRequest request)
			: base(message)
		{
			Request = request;
		}

		public VideoApiException(string message, IEnumerable<SearchResult> searchResult)
			: base(message)
		{
			SearchResults = searchResult;
		}

		public VideoApiException(string message, SearchResult searchResult)
			: base(message)
		{
			SearchResult = searchResult;
		}

		public VideoRequest? Request { get; }
		public IEnumerable<SearchResult>? SearchResults { get; }
		public SearchResult? SearchResult { get; }
	}
}