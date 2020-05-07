#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace GrainsInterfaces.CodecParser.Models
{
	public class FileInformation
	{
		public Container Container { get; set; }
		public string Title { get; set; } = string.Empty;
		public int ContainerVersion { get; set; }
		public TimeCodeScale TimeCodeScale { get; set; }
		public TimeSpan Duration { get; set; }
		public DateTime DateCreated { get; set; }
		public Guid SegmentId { get; set; }
		public Codec VideoCodec { get; set; }
		public int PixelHeight { get; set; }
		public int PixelWidth { get; set; }

		public IEnumerable<AudioTrack> Audios { get; set; }
			= Enumerable.Empty<AudioTrack>();

		public IEnumerable<Subtitle>? Subtitles { get; set; }

		public string Resolution => GetResolution();
		public double AspectRatio => GetAspectRatio();

		private double GetAspectRatio() => (double) PixelWidth / PixelHeight;

		private string GetResolution()
		{
			return (PixelWidth, PixelHeight) switch
			       {
				       (1920, 1080) => "1080p",
				       _            => $"{PixelWidth}x{PixelHeight}"
			       };
		}
	}
}