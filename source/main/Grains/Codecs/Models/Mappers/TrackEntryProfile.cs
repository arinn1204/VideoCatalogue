using AutoMapper;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks;
using Grains.Codecs.Models.Extensions;
using GrainsInterfaces.CodecParser.Models;

namespace Grains.Codecs.Models.Mappers
{
	public class TrackEntryProfile : Profile
	{
		public TrackEntryProfile()
		{
			CreateMap<TrackEntry, AudioTrack>()
			   .ForMember(
					dest => dest.Channels,
					src => src.MapFrom(m => m.AudioSettings!.Channels))
			   .ForMember(
					dest => dest.Frequency,
					src => src.MapFrom(m => m.AudioSettings!.SamplingFrequency))
			   .ForMember(
					dest => dest.Language,
					src => src.MapFrom(m => GetLanguage(m)))
			   .ForMember(
					dest => dest.Name,
					src => src.MapFrom(m => m.Name))
			   .ForMember(
					dest => dest.Codec,
					src => src.MapFrom(m => m.CodecId.ToCodec()));

			CreateMap<TrackEntry, Subtitle>()
			   .ForMember(
					dest => dest.Language,
					src => src.MapFrom(m => GetLanguage(m)))
			   .ForMember(
					dest => dest.Name,
					src => src.MapFrom(m => m.Name));
		}

		private string? GetLanguage(TrackEntry entry)
			=> string.IsNullOrWhiteSpace(entry.LanguageOverride)
				? entry.Language
				: entry.LanguageOverride;
	}
}