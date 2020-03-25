﻿using System.Collections.Generic;
using System.Linq;
using GrainsInterfaces.Models.CodecParser;

namespace Grains.Codecs.Matroska.Models
{
	public class SegmentInformation
	{
		public SegmentInformation()
		{
			Videos = Enumerable.Empty<VideoInformation>();
			Audios = Enumerable.Empty<AudioInformation>();
			Subtitles = Enumerable.Empty<Subtitle>();
		}

		public IEnumerable<VideoInformation> Videos { get; set; }
		public IEnumerable<AudioInformation> Audios { get; set; }
		public IEnumerable<Subtitle> Subtitles { get; set; }
	}
}