using System;
using System.Linq;
using FluentAssertions;
using Grains.Tests.Integration.Features.Models;
using GrainsInterfaces.BitTorrentClient.Models;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Assertions
{
	[Binding]
	public class BitTorrentAssertion
	{
		private readonly BitTorrentData _data;

		public BitTorrentAssertion(BitTorrentData data)
		{
			_data = data;
		}

		[Then(
			@"the client sees (\d+) (active|queued|paused|stopped) (seeding |downloading )?torrents?")]
		public void ThenTheClientSeesSeedingTorrents(
			int numberOfTorrents,
			string state,
			string activity)
		{
			switch (state)
			{
				case "active":
					Enum.TryParse(activity.Trim(' '), true, out TorrentStatus status);
					_data.Response
					     .Count(w => w.Status == status)
					     .Should()
					     .Be(numberOfTorrents);
					break;

				case "queued":
					Enum.TryParse(
						activity.Replace("ing", string.Empty).Trim(' '),
						true,
						out QueuedStatus queuedStatus);
					_data.Response
					     .Count(
						      w => w.Status == TorrentStatus.Queued &&
						           w.QueuedStatus == queuedStatus)
					     .Should()
					     .Be(numberOfTorrents);
					break;

				case "paused":
				case "stopped":
					_data.Response
					     .Count(w => w.Status == TorrentStatus.Stopped)
					     .Should()
					     .Be(numberOfTorrents);
					break;
			}
		}
	}
}