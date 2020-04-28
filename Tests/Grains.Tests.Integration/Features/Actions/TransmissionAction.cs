﻿using System.Linq;
using System.Threading.Tasks;
using GrainsInterfaces.BitTorrent;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Actions
{
	[Binding]
	public class TransmissionAction
	{
		private readonly IBitTorrentClient _bitTorrentClient;

		public TransmissionAction(IBitTorrentClient bitTorrentClient)
		{
			_bitTorrentClient = bitTorrentClient;
		}

		[When(@"the client goes to view the active torrents")]
		public async Task TheClientGoesToViewActiveTorrents()
		{
			var activeTorrents = await _bitTorrentClient.GetActiveTorrents();

			var names = await activeTorrents.ToListAsync();
		}
	}
}