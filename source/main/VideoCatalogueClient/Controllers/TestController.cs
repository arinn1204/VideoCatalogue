﻿using GrainsInterfaces.BitTorrentClient;
using GrainsInterfaces.CodecParser;
using GrainsInterfaces.VideoApi;
using GrainsInterfaces.VideoLocator;
using Microsoft.AspNetCore.Mvc;

namespace VideoCatalogueClient.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Produces("application/json")]
	public class TestController : Controller
	{
		private readonly IBitTorrentClient _btClient;
		private readonly IParser _parser;
		private readonly ISearcher _searcher;
		private readonly IVideoApi _videoApi;

		public TestController(
			IBitTorrentClient btClient,
			IParser parser,
			IVideoApi videoApi,
			ISearcher searcher)
		{
			_btClient = btClient;
			_parser = parser;
			_videoApi = videoApi;
			_searcher = searcher;
		}

		[HttpGet]
		public void Foo()
		{
		}
	}
}