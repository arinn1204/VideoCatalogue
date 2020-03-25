﻿using System;
using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren
{
	public class Chapter : ISegmentChild
	{
#region ISegmentChild Members

		public Segment Merge(
			Segment totalParent,
			object childInformation)
			=> throw new NotImplementedException();

		public object GetChildInformation(Stream stream, EbmlSpecification specification)
			=> throw new NotImplementedException();

#endregion
	}
}