#nullable enable
using System;
using System.Collections.Generic;

namespace GrainsInterfaces.Models.CodecParser
{
	public class FileInformation
	{
		public string Resolution => GetResolution();
		public Container Container { get; set; }
		public double Duration { get; set; }
		public DateTime? DateCreated { get; set; }
		public Guid SegmentId { get; set; }
		public Codec VideoCodec { get; set; }
		public int PixelHeight { get; set; }
		public int PixelWidth { get; set; }
		public IEnumerable<AudioTrack> Audios { get; set; }
		public IEnumerable<Subtitle>? Subtitles { get; set; }

		private string GetResolution()
		{
			return (PixelHeight, PixelWidth) switch
			       {
				       (1920, 1080) => "1080p",
				       _            => $"{PixelHeight}x{PixelWidth}"
			       };
		}
	}
}