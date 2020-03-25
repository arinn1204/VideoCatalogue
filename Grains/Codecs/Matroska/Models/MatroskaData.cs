using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;

namespace Grains.Codecs.Matroska.Models
{
	public class MatroskaData
	{
		public EbmlHeaderData Header { get; set; }
		public Segment Segment { get; set; }
	}
}