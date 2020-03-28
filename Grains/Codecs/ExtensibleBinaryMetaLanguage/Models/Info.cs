#nullable enable
using System;
using Grains.Codecs.Models.AlignedModels;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models
{
	public class Info
	{
		public uint? SegmentUid { get; set; }
		public string? SegmentFilename { get; set; }
		public uint? PreviousUid { get; set; }
		public uint? PreviousFilename { get; set; }
		public uint? NextUid { get; set; }
		public uint? NextFilename { get; set; }
		public uint? SegmentFamily { get; set; }
		public uint TimestampScale { get; set; }
		public Float32? Duration { get; set; }
		public DateTime? DateUtc { get; set; }
		public string? Title { get; set; }
		public string MuxingApp { get; set; }
		public string WritingApp { get; set; }
		public ChapterTranslate ChapterTranslate { get; set; }
	}
}