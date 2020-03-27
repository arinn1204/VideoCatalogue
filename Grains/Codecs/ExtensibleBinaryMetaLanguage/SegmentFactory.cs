using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Exceptions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage
{
	public class SegmentFactory : ISegmentFactory
	{
		private readonly IReader _reader;

		public SegmentFactory(IReader reader)
		{
			_reader = reader;
		}

#region ISegmentFactory Members

		public ISegmentChild GetChild(string name)
		{
			return name switch
			       {
				       "SeekHead"    => new SeekHeadReader(_reader),
				       "Info"        => new Info(),
				       "Tracks"      => new Track(),
				       "Chapters"    => new Chapter(),
				       "Cluster"     => new Cluster(),
				       "Cues"        => new Cue(),
				       "Attachments" => new Attachment(),
				       "Tags"        => new Tag(),
				       _ => throw new UnsupportedException(
					       $"'{name}' is not a supported segment child name.")
			       };
		}

#endregion
	}
}