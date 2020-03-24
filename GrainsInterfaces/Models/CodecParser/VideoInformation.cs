using System.Collections.Generic;

namespace GrainsInterfaces.Models.CodecParser
{
	public class VideoInformation
	{
		public Codec VideoCodec { get; set; }
		public string Title { get; set; }
		public double Duration { get; set; }
		public IEnumerable<Chapter> Chapters { get; set; }
		public int Height { get; set; }
		public int Width { get; set; }
		public string Resolution => GetResolution();

		private string GetResolution()
		{
			return (Height, Width) switch
			       {
				       (1920, 1080) => "1080p",
				       _            => $"{Height}x{Width}"
			       };
		}
	}
}