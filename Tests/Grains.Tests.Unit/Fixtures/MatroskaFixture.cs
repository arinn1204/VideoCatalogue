using System;
using System.IO;
using System.Xml.Serialization;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;

namespace Grains.Tests.Unit.Fixtures
{
	public class MatroskaFixture : IDisposable
	{
		public MatroskaFixture()
		{
			var serializer = new XmlSerializer(typeof(EbmlSpecification));
			using var content = new FileStream(
				Path.Combine("TestData", "Matroska", "Specification.xml"),
				FileMode.Open,
				FileAccess.Read);
			Specification = (EbmlSpecification) serializer.Deserialize(content);
		}

		public EbmlSpecification Specification { get; }

#region IDisposable Members

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

#endregion
	}
}