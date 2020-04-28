using System.Linq;
using System.Threading.Tasks;
using Grains.Tests.Integration.Features.Models;
using GrainsInterfaces.BitTorrent;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Actions
{
	[Binding]
	public class BitTorrentAction
	{
		private readonly IBitTorrentClient _bitTorrentClient;
		private readonly BitTorrentData _data;

		public BitTorrentAction(
			IBitTorrentClient bitTorrentClient,
			BitTorrentData data)
		{
			_bitTorrentClient = bitTorrentClient;
			_data = data;
		}

		[When(@"the client goes to view the active torrents")]
		public async Task TheClientGoesToViewActiveTorrents()
		{
			var activeTorrents = await _bitTorrentClient.GetActiveTorrents();
			var response = await activeTorrents.ToListAsync();
			_data.Response = response;
		}
	}
}