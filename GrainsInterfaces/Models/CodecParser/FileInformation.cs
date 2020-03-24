using System.Collections.Generic;

namespace GrainsInterfaces.Models.CodecParser
{
	public class FileInformation
	{
		public uint Id { get; set; }
		public string Container { get; set; }
		public int EbmlVersion { get; set; }
		public IEnumerable<VideoInformation> Videos { get; set; }
		public IEnumerable<AudioInformation> Audios { get; set; }
		public IEnumerable<Subtitle> Subtitles { get; set; }
	}
}