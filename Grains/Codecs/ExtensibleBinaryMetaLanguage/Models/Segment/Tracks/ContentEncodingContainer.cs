﻿using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks
{
	[EbmlMaster("ContentEncodings")]
	public class ContentEncodingContainer
	{
		[EbmlElement("ContentEncodings")]
		public IEnumerable<ContentEncoding>? ContentEncodingSettings { get; set; }
	}
}