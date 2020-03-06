using GrainsInterfaces.Models.VideoApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grains.VideoApi.Models.VideoApi.Exceptions
{
    public class VideoApiException : Exception
    {
        public VideoApiException(string message, VideoRequest request) : base(message)
        {
            Request = request;
        }

        public VideoApiException(string message, IEnumerable<SearchResult> searchResult) : base(message)
        {
            SearchResults = searchResult;
        }

        public VideoApiException(string message, SearchResult searchResult) : base(message)
        {
            SearchResult = searchResult;
        }

        public VideoRequest Request { get; }
        public IEnumerable<SearchResult> SearchResults { get; }
        public SearchResult SearchResult { get; }
    }
}
