namespace Grains.Codecs.ExtensibleBinaryMetaLanguage
{
	public class SegmentFactory : ISegmentFactory
	{
		public ISegmentChild GetChild(string name)
		{
			return name switch
			       {
				       "SeekHead"    => new SeekHead(),
				       "Info"        => new SeekHead(),
				       "Tracks"      => new SeekHead(),
				       "Chapters"    => new SeekHead(),
				       "Cluster"     => new SeekHead(),
				       "Cues"        => new SeekHead(),
				       "Attachments" => new SeekHead(),
				       "Tags"        => new SeekHead(),
				       _             => new SeekHead()
			       };
		}
	}
}