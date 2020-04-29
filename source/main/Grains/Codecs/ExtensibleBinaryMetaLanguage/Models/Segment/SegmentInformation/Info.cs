#nullable enable
using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SegmentInformation
{
	[EbmlMaster]
	public class Info
	{
		[EbmlElement("SegmentUID")]
		public byte[]? SegmentUid { get; set; }

		public string? SegmentFilename { get; set; }

		[EbmlElement("PrevUID")]
		public byte[]? PreviousSegmentUid { get; set; }

		[EbmlElement("PrevFilename")]
		public string? PreviousSegmentFilename { get; set; }

		[EbmlElement("NextUID")]
		public byte[]? NextSegmentUid { get; set; }

		[EbmlElement("NextFilename")]
		public string? NextSegmentFilename { get; set; }

		public byte[]? SegmentFamily { get; set; }
		public uint TimecodeScale { get; set; }
		public float? Duration { get; set; }

		/// <summary>
		///     The time in nanoseconds (per scale) since January 1, 2001
		/// </summary>
		[EbmlElement("DateUTC")]
		public ulong? TimeSinceMatroskaEpoch { get; set; }

		public string? Title { get; set; }
		public string MuxingApp { get; set; } = string.Empty;
		public string WritingApp { get; set; } = string.Empty;

		[EbmlElement("ChapterTranslate")]
		public IEnumerable<ChapterTranslate>? ChapterTranslates { get; set; }
	}
}