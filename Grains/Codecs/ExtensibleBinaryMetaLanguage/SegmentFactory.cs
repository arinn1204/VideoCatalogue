using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Exceptions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage
{
	public class SegmentFactory : ISegmentFactory
	{
#region ISegmentFactory Members

		public ISegmentChild GetChild(string name)
		{
			return name switch
			       {
				       "SeekHead"    => new SeekHead(),
				       "Info"        => new Info(),
				       "Tracks"      => new Tracks(),
				       "Chapters"    => new Chapters(),
				       "Cluster"     => new Cluster(),
				       "Cues"        => new Cues(),
				       "Attachments" => new Attachments(),
				       "Tags"        => new Tags(),
				       _ => throw new UnsupportedException(
					       $"{name} is not a supported segment child name.")
			       };
		}

#endregion
	}
}