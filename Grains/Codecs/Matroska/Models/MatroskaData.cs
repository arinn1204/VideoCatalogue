using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment;

namespace Grains.Codecs.Matroska.Models
{
	public class MatroskaData
	{
		public EbmlHeaderData Header { get; set; }
		public Segment Segment { get; set; }
	}
}