#nullable enable
using System;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models
{
	public class Info
	{
		public ulong? SegmentUID { get; set; }
		public string? SegmentFilename { get; set; }
		public ulong? PrevUID { get; set; }
		public string? PrevFilename { get; set; }
		public ulong? NextUID { get; set; }
		public string? NextFilename { get; set; }
		public ulong? SegmentFamily { get; set; }
		public uint TimecodeScale { get; set; }
		public float? Duration { get; set; }
		public DateTime? DateUTC { get; set; }
		public string? Title { get; set; }
		public string MuxingApp { get; set; }
		public string WritingApp { get; set; }
		public ChapterTranslate? ChapterTranslate { get; set; }
	}
}