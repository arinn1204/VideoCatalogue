using System.Collections.Generic;
using GrainsInterfaces.Models.CodecParser;

namespace Grains.Codecs.Matroska.Models
{
	public class SegmentInformation
	{
		public IEnumerable<VideoInformation> Videos { get; set; }
		public IEnumerable<AudioInformation> Audios { get; set; }
		public IEnumerable<Subtitle> Subtitles { get; set; }
	}
}