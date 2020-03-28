using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Tracks
{
	[EbmlMaster]
	public class TrackEntry
	{
		public string Name { get; set; }
		public uint TrackNumber { get; set; }
		public uint TrackType { get; set; }
	}
}