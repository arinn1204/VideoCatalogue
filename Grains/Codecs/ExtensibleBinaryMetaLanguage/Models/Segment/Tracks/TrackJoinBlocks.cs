﻿using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks
{
	[EbmlMaster]
	public class TrackJoinBlocks
	{
		[EbmlElement("TrackJoinUID")]
		public IEnumerable<uint> TrackJoinUids { get; set; }
	}
}