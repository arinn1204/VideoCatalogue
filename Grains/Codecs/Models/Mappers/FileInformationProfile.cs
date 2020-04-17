using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
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
			CreateMap<EbmlDocument, FileInformation>()
			   .ForMember(
					dest => dest.Container,
					src => src.MapFrom(m => GetContainerName(m)))
			   .ForMember(
					dest => dest.ContainerVersion,
					src => src.MapFrom(
						m => m.EbmlHeader
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
						m => m.Segment
						      .SegmentInformations
						      .DistinctBy(d => new Guid(d.SegmentUid))
						      .Select(s => new Guid(s.SegmentUid))
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

		private TimeSpan GetDuration(EbmlDocument documents)
		{
			var info = GetInfo(documents);
			var duration = info.Duration.HasValue
				? info.Duration.Value
				: 0.0;
			var milliseconds = info.TimecodeScale.ToTimeCodeScale().ToMilliseconds(duration);

			return new TimeSpan(
				0,
				0,
				0,
				0,
				(int) milliseconds);
		}

		private DateTime GetDateTime(EbmlDocument documents)
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

			var nanoSeconds = info.TimeSinceMatroskaEpoch.HasValue
				? info.TimeSinceMatroskaEpoch.Value
				: 0.0;

			var milliseconds = nanoSeconds / (int) info.TimecodeScale.ToTimeCodeScale();
			return baseDate.AddMilliseconds(milliseconds);
		}

		private Info GetInfo(EbmlDocument documents)
		{
			return documents
			      .Segment
			      .SegmentInformations
			      .DistinctBy(d => new Guid(d.SegmentUid))
			      .First();
		}

		private TrackEntry GetVideoTrack(EbmlDocument documents)
			=> GetTracks(documents, "V").First();

		private IEnumerable<TrackEntry> GetAudioTracks(EbmlDocument documents)
			=> GetTracks(documents, "A");

		private IEnumerable<TrackEntry> GetSubtitles(EbmlDocument documents)
			=> GetTracks(documents, "S");

		private IEnumerable<TrackEntry> GetTracks(
			EbmlDocument documents,
			string codecStartsWith)
		{
			return documents.Segment
			                .Tracks
			                .SelectMany(s1 => s1.Entries)
			                .Where(w => w.CodecId.StartsWith(codecStartsWith));
		}

		private Container GetContainerName(EbmlDocument documents)
			=> documents.EbmlHeader.DocType.ToContainer();
	}
}