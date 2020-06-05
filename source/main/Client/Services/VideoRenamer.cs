using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues.Models;
using Client.Services.Interfaces;
using GrainsInterfaces.BitTorrentClient;
using GrainsInterfaces.CodecParser;
using GrainsInterfaces.VideoApi;
using GrainsInterfaces.VideoFilter;
using GrainsInterfaces.VideoLocator;

namespace Client.Services
{
	public class VideoRenamer : IVideoRenamer
	{
		private readonly IBitTorrentClient _btClient;
		private readonly IParser _parser;
		private readonly ISearcher _searcher;
		private readonly IVideoApi _videoApi;
		private readonly IVideoFilter _videoFilter;

		public VideoRenamer(
			IBitTorrentClient bitTorrentClient,
			IParser parser,
			IVideoApi videoApi,
			IVideoFilter videoFilter,
			ISearcher searcher)
			=> (_btClient, _parser, _videoApi, _videoFilter, _searcher) = (
				bitTorrentClient, parser, videoApi, videoFilter, searcher);

#region IVideoRenamer Members

		public async Task<IEnumerable<RenamerResult>> ProcessMessage(
			IEnumerable<QueueMessage> message,
			CancellationToken cancellationToken)
		{
			await Task.CompletedTask;

			return Enumerable.Empty<RenamerResult>();
		}

#endregion
	}
}