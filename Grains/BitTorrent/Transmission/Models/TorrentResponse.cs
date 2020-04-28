using System.Collections.Generic;
using System.Linq;

namespace Grains.BitTorrent.Transmission.Models
{
	public class TorrentResponse
	{
		public IEnumerable<FileResponse> Files { get; set; }
			= Enumerable.Empty<FileResponse>();

		public string Name { get; set; }
			= string.Empty;

		public int Status { get; set; }
	}
}