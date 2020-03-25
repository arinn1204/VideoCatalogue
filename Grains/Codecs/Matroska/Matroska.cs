using System;
using System.IO;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.Matroska.Interfaces;
using Grains.Codecs.Matroska.Models;
using GrainsInterfaces.Models.CodecParser;

namespace Grains.Codecs.Matroska
{
	public class Matroska : IMatroska
	{
		private readonly Lazy<(uint ebml, uint segment)> _ebmlAndSegmentId;
		private readonly IEbmlHeader _ebmlHeader;
		private readonly Lazy<EbmlSpecification> _matroskaSpecification;
		private readonly ISegment _segment;

		public Matroska(
			ISpecification specification,
			IEbmlHeader ebmlHeader,
			ISegment segment)
		{
			_ =
				specification ?? throw new ArgumentNullException(nameof(specification));
			_ebmlHeader = ebmlHeader;
			_segment = segment;
			_matroskaSpecification =
				new Lazy<EbmlSpecification>(
					() => specification.GetSpecification()
					                   .ConfigureAwait(false)
					                   .GetAwaiter()
					                   .GetResult());

			_ebmlAndSegmentId = new Lazy<(uint ebml, uint segment)>(
				() =>
				{
					var values = _matroskaSpecification.Value
					                                   .Elements
					                                   .Where(f => f.Level == 0)
					                                   .ToList();

					return (values.First(f => f.Name == "EBML")
					              .Id,
					        values.First(f => f.Name == "Segment")
					              .Id);
				});
		}

#region IMatroska Members

		public bool IsMatroska(Stream stream)
		{
			var id = _ebmlHeader.GetMasterIds(stream, _matroskaSpecification.Value);

			if (id != _ebmlAndSegmentId.Value.ebml)
			{
				return false; //not EBML marked, all matroska will be
			}

			var header = _ebmlHeader.GetHeaderInformation(stream, _matroskaSpecification.Value);

			return header.DocType == "matroska";
		}

		public FileInformation GetFileInformation(Stream stream, out MatroskaError error)
		{
			error = null;
			var id = _ebmlHeader.GetMasterIds(stream, _matroskaSpecification.Value);

			if (id != _ebmlAndSegmentId.Value.ebml)
			{
				error = new MatroskaError($"{id} is not a valid ebml ID.");
				return default;
			}

			var ebmlHeader = _ebmlHeader.GetHeaderInformation(stream, _matroskaSpecification.Value);

			if (ebmlHeader.Version != 1 || ebmlHeader.DocType != "matroska")
			{
				var errorDescription = ebmlHeader.Version != 1
					? $"Ebml version of '{ebmlHeader.Version}' is not supported."
					: $"Ebml doctype of '{ebmlHeader.DocType}' is not supported.";
				error = new MatroskaError(errorDescription);
				return default;
			}

			var segmentInformation = new SegmentInformation();

			while ((id = _ebmlHeader.GetMasterIds(stream, _matroskaSpecification.Value)) ==
			       _ebmlAndSegmentId.Value.segment)
			{
				var segment = _segment.GetSegmentInformation(
					stream,
					_matroskaSpecification.Value);

				segmentInformation.Audios = segmentInformation.Audios.Concat(segment.Audios);
				segmentInformation.Videos = segmentInformation.Videos.Concat(segment.Videos);
				segmentInformation.Subtitles =
					segmentInformation.Subtitles.Concat(segment.Subtitles);
			}

			return new FileInformation
			       {
				       Container = ebmlHeader.DocType,
				       Audios = segmentInformation.Audios,
				       Subtitles = segmentInformation.Subtitles,
				       Videos = segmentInformation.Videos,
				       EbmlVersion = (int) ebmlHeader.Version
			       };
		}

#endregion
	}
}