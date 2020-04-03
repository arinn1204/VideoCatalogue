#nullable enable
using System;
using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SegmentInformation
{
	[EbmlMaster]
	public class Info
	{
		[EbmlElement("SegmentUID")]
		public byte[]? SegmentUID { get; set; }

		public string? SegmentFilename { get; set; }

		[EbmlElement("PrevUID")]
		public byte[]? PrevUID { get; set; }

		[EbmlElement("PrevFilename")]
		public string? PrevFilename { get; set; }

		[EbmlElement("NextUID")]
		public byte[]? NextUID { get; set; }

		public string? NextFilename { get; set; }
		public byte[]? SegmentFamily { get; set; }
		public uint TimecodeScale { get; set; }
		public float? Duration { get; set; }

		[EbmlElement("DateUTC")]
		public DateTime? DateUTC { get; set; }

		public string? Title { get; set; }
		public string MuxingApp { get; set; }
		public string WritingApp { get; set; }
		public IEnumerable<ChapterTranslate>? ChapterTranslates { get; set; }
	}
}