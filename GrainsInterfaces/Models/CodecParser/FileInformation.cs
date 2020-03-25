using System.Collections.Generic;

namespace GrainsInterfaces.Models.CodecParser
{
	public class FileInformation
	{
		public string Container { get; set; }
		public int EbmlVersion { get; set; }
		public IEnumerable<VideoInformation> Videos { get; set; }
		public IEnumerable<AudioInformation> Audios { get; set; }
		public IEnumerable<Subtitle> Subtitles { get; set; }
	}
}