namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models
{
	public class EbmlDocument
	{
		public EbmlHeader? EbmlHeader { get; set; }
		public Segment.Segment? Segment { get; set; }
	}
}