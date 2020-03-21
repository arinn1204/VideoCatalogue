using System.Collections.Generic;

namespace GrainsInterfaces.Models.CodecParser
{
	public class VideoInformation
	{
		public Codec VideoCodec { get; set; }
		public string Title { get; set; }
		public double Duration { get; set; }
		public string Resolution { get; set; }
		public IEnumerable<Chapter> Chapters { get; set; }
	}
}