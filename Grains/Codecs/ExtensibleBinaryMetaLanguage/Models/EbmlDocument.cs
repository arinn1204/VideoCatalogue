namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models
{
	public class EbmlDocument
	{
		public EbmlHeaderData EbmlHeader { get; set; }
		public Segment.Segment Segment { get; set; }
	}
}