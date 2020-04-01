using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Extensions
{
	public static class EbmlSpecificationExtensions
	{
		public static IEnumerable<uint> GetSkippableElements(this EbmlSpecification @this)
			=> @this.Elements.Where(
				         w =>
					         (w.Name == "Void") |
					         (w.Name == "CRC-32") |
					         w.Name.Contains("Private") |
					         (w.Name == "Block") |
					         (w.Name == "SimpleBlock") |
					         (w.Name == "BlockAdditional") |
					         (w.Name == "CodecState") |
					         (w.Name == "SignaturePublicKey") |
					         (w.Name == "BlockVirtual") |
					         (w.Name == "EncryptedBlock") |
					         (w.Name == "ContentEncKeyID") |
					         (w.Name == "ContentCompSettings") |
					         (w.Name == "ContentSigKeyID") |
					         (w.Name == "FileData") |
					         (w.Name == "FileReferral") |
					         (w.Name == "ChapProcessPrivate") |
					         (w.Name == "ChapProcessData") |
					         (w.Name == "TagBinary") |
					         (w.Name == "Cluster") |
					         (w.Name == "Cueing Data") |
					         (w.Name == "Attachments") |
					         (w.Name == "Tags"))
			        .Select(s => s.Id);

		public static IReadOnlyDictionary<uint, EbmlElement> GetInfoElements(
			this EbmlSpecification @this)
			=> GetElements(@this, "Info");

		public static IReadOnlyDictionary<uint, EbmlElement> GetTrackElements(
			this EbmlSpecification @this)
			=> GetElements(@this, "Tracks");

		private static IReadOnlyDictionary<uint, EbmlElement> GetElements(
			EbmlSpecification specification,
			string elementName,
			int level = 1)
		{
			return specification.Elements
			                    .SkipUntil(t => t.Name == elementName)
			                    .TakeWhile(t => t.Level != level)
			                    .ToDictionary(k => k.Id);
		}
	}
}