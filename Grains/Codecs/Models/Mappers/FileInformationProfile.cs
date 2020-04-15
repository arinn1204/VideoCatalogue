using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Exceptions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SegmentInformation;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks;
using Grains.Codecs.Models.Extensions;
using GrainsInterfaces.Models.CodecParser;
using static MoreLinq.Extensions.DistinctByExtension;

namespace Grains.Codecs.Models.Mappers
{
	public class FileInformationProfile : Profile
	{
		public FileInformationProfile()
		{
			CreateMap<IEnumerable<EbmlDocument>, FileInformation>()
			   .ForMember(
					dest => dest.Container,
					src => src.MapFrom(m => GetContainerName(m)))
			   .ForMember(
					dest => dest.ContainerVersion,
					src => src.MapFrom(
						m => m.DistinctBy(d => d.EbmlHeader.DocTypeVersion)
						      .Single()
						      .EbmlHeader
						      .DocTypeVersion))
			   .ForMember(
					dest => dest.Audios,
					src => src.MapFrom(m => GetAudioTracks(m)))
			   .ForMember(
					dest => dest.Subtitles,
					src => src.MapFrom(m => GetSubtitles(m)))
			   .ForMember(
					dest => dest.Duration,
					src => src.MapFrom(m => GetDuration(m)))
			   .ForMember(
					dest => dest.DateCreated,
					src => src.MapFrom(m => GetDateTime(m)))
			   .ForMember(
					dest => dest.SegmentId,
					src => src.MapFrom(
						m => GetSegment(m)
						    .SegmentInformations
						    .DistinctBy(d => new Guid(d.SegmentUID))
						    .Select(s => new Guid(s.SegmentUID))
						    .First()))
			   .ForMember(
					dest => dest.TimeCodeScale,
					src => src.MapFrom(m => GetInfo(m).TimecodeScale.ToTimeCodeScale()))
			   .ForMember(
					dest => dest.VideoCodec,
					src => src.MapFrom(m => GetVideoTrack(m).CodecId.ToCodec()))
			   .ForMember(
					dest => dest.PixelHeight,
					src => src.MapFrom(m => GetVideoTrack(m).VideoSettings.PixelHeight))
			   .ForMember(
					dest => dest.PixelWidth,
					src => src.MapFrom(m => GetVideoTrack(m).VideoSettings.PixelWidth))
			   .ForMember(
					dest => dest.Title,
					src => src.MapFrom(m => GetInfo(m).Title));
		}

		private TimeSpan GetDuration(IEnumerable<EbmlDocument> documents)
		{
			var info = GetInfo(documents);
			var scale = info.TimecodeScale.ToTimeCodeScale();
			var duration = info.Duration.HasValue
				? info.Duration.Value
				: 0.0;

			return scale switch
			       {
				       TimeCodeScale.Millisecond => new TimeSpan(
					       0,
					       0,
					       0,
					       0,
					       (int) duration),

				       _ => throw new UnsupportedException($"{scale} is not supported.")
			       };
		}

		private DateTime GetDateTime(IEnumerable<EbmlDocument> documents)
		{
			var info = GetInfo(documents);
			var baseDate = new DateTime(
				2001,
				1,
				1,
				0,
				0,
				0,
				DateTimeKind.Utc);

			var secondsInScale = info.DateUTC.HasValue
				? info.DateUTC.Value
				: 0.0;

			var milliseconds = secondsInScale / (int) info.TimecodeScale.ToTimeCodeScale();

			return baseDate.AddMilliseconds(milliseconds);
		}

		private Info GetInfo(IEnumerable<EbmlDocument> documents)
		{
			return GetSegment(documents)
			      .SegmentInformations
			      .DistinctBy(d => new Guid(d.SegmentUID))
			      .First();
		}

		private Segment GetSegment(IEnumerable<EbmlDocument> documents)
		{
			return documents.DistinctBy(d => d.EbmlHeader)
			                .Select(f => f.Segment)
			                .First();
		}

		private TrackEntry GetVideoTrack(IEnumerable<EbmlDocument> documents)
			=> GetTracks(documents, "V").First();

		private IEnumerable<TrackEntry> GetAudioTracks(IEnumerable<EbmlDocument> documents)
			=> GetTracks(documents, "A");

		private IEnumerable<TrackEntry> GetSubtitles(IEnumerable<EbmlDocument> documents)
			=> GetTracks(documents, "S");

		private IEnumerable<TrackEntry> GetTracks(
			IEnumerable<EbmlDocument> documents,
			string codecStartsWith)
		{
			return documents.SelectMany(
				s => s.Segment.Tracks.SelectMany(s1 => s1.TrackEntries)
				      .Where(w => w.CodecId.StartsWith(codecStartsWith)));
		}

		private Container GetContainerName(IEnumerable<EbmlDocument> documents)
		{
			var containerName = documents.DistinctBy(d => d.EbmlHeader.DocType)
			                             .Single();

			return Enum.TryParse(
				       containerName.EbmlHeader.DocType,
				       true,
				       out Container result) switch
			       {
				       true  => result,
				       false => Container.Unknown
			       };
		}
	}
}