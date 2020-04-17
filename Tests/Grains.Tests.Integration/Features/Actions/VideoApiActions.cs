using System;
using System.Threading.Tasks;
using BoDi;
using Grains.Tests.Integration.Extensions.Wiremock;
using GrainsInterfaces.Models.VideoApi;
using GrainsInterfaces.VideoApi;
using TechTalk.SpecFlow;
using WireMock.Server;

namespace Grains.Tests.Integration.Features.Actions
{
	[Binding]
	public class VideoApiActions
	{
		private readonly IObjectContainer _container;
		private readonly VideoRequest _request;
		private readonly IVideoApi _videoApi;

		public VideoApiActions(
			IVideoApi videoApi,
			VideoRequest request,
			IObjectContainer container)
		{
			_videoApi = videoApi;
			_request = request;
			_container = container;
		}

		[When(@"the client requests movie details")]
		public async Task WhenTheClientRequestsMovieDetails()
		{
			var videoId = GetVideoId(_request.Title);
			var wiremockServer = _container.Resolve<WireMockServer>();
			wiremockServer.AddSearch(_request)
			              .AddMovieDetail(_request.Title, videoId)
			              .AddMovieCredit(_request.Title, videoId);

			wiremockServer.LogEntriesChanged += (sender, args) =>
			                                    {
				                                    var wm = wiremockServer;
				                                    var r = _request;
			                                    };

			var details = await _videoApi.GetVideoDetails(_request);
			_container.RegisterInstanceAs(details);
		}

		private int GetVideoId(string title)
		{
			return title switch
			       {
				       "The Avengers" => 24428,
				       _ => throw new Exception(
					       $"{title} has not been setup with an id")
			       };
		}
	}
}