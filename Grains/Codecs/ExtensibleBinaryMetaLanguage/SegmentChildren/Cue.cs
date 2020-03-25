using System;
using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren
{
	public class Cue : ISegmentChild
	{
#region ISegmentChild Members

		public SegmentInformation Merge(
			SegmentInformation totalParentInformation,
			object childInformation)
			=> throw new NotImplementedException();

		public object GetChildInformation(Stream stream, EbmlSpecification specification)
			=> throw new NotImplementedException();

#endregion
	}
}