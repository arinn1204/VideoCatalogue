#nullable enable
using System;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models
{
	[EbmlMaster]
	public class Info
	{
		[EbmlElement("SegmentUID")]
		public ulong? SegmentUID { get; set; }

		public string? SegmentFilename { get; set; }

		[EbmlElement("PrevUID")]
		public ulong? PrevUID { get; set; }

		[EbmlElement("PrevFilename")]
		public string? PrevFilename { get; set; }

		[EbmlElement("NextUID")]
		public ulong? NextUID { get; set; }

		public string? NextFilename { get; set; }
		public ulong? SegmentFamily { get; set; }
		public uint TimecodeScale { get; set; }
		public float? Duration { get; set; }

		[EbmlElement("DateUTC")]
		public DateTime? DateUTC { get; set; }

		public string? Title { get; set; }
		public string MuxingApp { get; set; }
		public string WritingApp { get; set; }
		public ChapterTranslate? ChapterTranslate { get; set; }
	}
}