using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Grains.BitTorrent.Transmission.Models
{
	public class ResponseArguments
	{
		[JsonPropertyName("torrents")]
		public IEnumerable<TorrentResponse> TorrentResponses { get; set; }
			= Enumerable.Empty<TorrentResponse>();
	}
}