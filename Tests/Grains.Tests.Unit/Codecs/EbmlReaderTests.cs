using System.IO;
using System.Linq;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Extensions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SegmentInformation;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Specification;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers;
using Grains.Tests.Unit.Extensions;
using Grains.Tests.Unit.Fixtures;
using Moq;
using Xunit;

namespace Grains.Tests.Unit.Codecs
{
	public class EbmlReaderTests : IClassFixture<MatroskaFixture>
	{
#region Setup/Teardown

		public EbmlReaderTests(MatroskaFixture fixture)
		{
			_specification = fixture.Specification;
		}

#endregion

		private readonly EbmlSpecification _specification;

		[Fact]
		public void ShouldSeekIfReceivingSkippedId()
		{
			var stream = new MemoryStream();
			var reader = new Mock<EbmlReader>();
			var skippedId = _specification.Elements
			                              .First(f => f.Name == "Void")
			                              .IdString
			                              .ToBytes()
			                              .ToArray();

			reader.Setup(s => s.ReadBytes(stream, 1))
			      .Returns(skippedId);

			reader.Setup(s => s.GetSize(stream))
			      .Returns<Stream>(
				       s =>
				       {
					       s.Position++;
					       return 5;
				       });

			var result = reader.Object.GetElement<Info>(
				stream,
				1,
				_specification.Elements.ToDictionary(k => k.Id),
				_specification.GetSkippableElements().ToList());

			result.Should().BeEquivalentTo(new Info());

			reader.Verify(v => v.GetSize(stream), Times.Once);
			reader.Verify(v => v.ReadBytes(stream, 1), Times.Once);
		}
	}
}