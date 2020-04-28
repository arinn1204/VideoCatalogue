using System;
using System.Linq;
using Grains.Tests.Integration.Features.Models;
using GrainsInterfaces.BitTorrent.Models;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Builders
{
	[Binding]
	public class BitTorrentBuilder
	{
		private readonly BitTorrentData _data;

		public BitTorrentBuilder(BitTorrentData data)
		{
			_data = data;
		}

		[Given(@"(\d+) (active|queued|paused|stopped) (seeding |downloading )?torrents?")]
		public void GivenAnActiveSeedingTorrent(int numberOfTorrents, string state, string action)
		{
			switch (state)
			{
				case "active":
					Enum.TryParse(action.Trim(' '), true, out TorrentStatus status);
					_data.Status = _data.Status.Concat(Enumerable.Repeat(status, numberOfTorrents));
					break;

				case "queued":
					Enum.TryParse(
						action.Replace("ing", string.Empty).Trim(' '),
						true,
						out QueuedStatus queuedStatus);
					_data.Status = _data.Status.Concat(
						Enumerable.Repeat(
							TorrentStatus.Queued,
							numberOfTorrents));
					_data.QueuedStatus = _data.QueuedStatus.Concat(
						Enumerable.Repeat(
							queuedStatus,
							numberOfTorrents));
					break;

				case "paused":
				case "stopped":
					_data.Status = _data.Status.Concat(
						Enumerable.Repeat(
							TorrentStatus.Stopped,
							numberOfTorrents));
					break;
			}
		}
	}
}