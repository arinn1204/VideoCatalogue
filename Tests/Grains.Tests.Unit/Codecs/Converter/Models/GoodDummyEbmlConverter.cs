using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Tests.Unit.Codecs.Converter.Models
{
	[EbmlMaster]
	public class GoodDummyEbmlConverter
	{
		[EbmlElement("Duplicate")]
		public bool IsEnabled { get; set; }

		[EbmlElement("NotDuplicate")]
		public string NotDuplicate { get; set; }

		public ulong? ThisValueIsALong { get; set; }

		public int[] Array { get; set; }
		public List<int> List { get; set; }
		public IEnumerable<int> Enumerable { get; set; }
		public IList<int> IList { get; set; }
	}
}