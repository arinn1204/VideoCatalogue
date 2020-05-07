using System.Linq;
using AutoMapper;
using Grains.VideoInformation.Models.Details;
using GrainsInterfaces.VideoApi.Models;

namespace Grains.VideoInformation.Models.Profiles
{
	public class VideoDetailProfile : Profile
	{
		public VideoDetailProfile()
		{
			CreateMap<MovieDetail, VideoDetail>()
			   .ForMember(dest => dest.Credits, src => src.Ignore())
			   .ForMember(dest => dest.ImdbId, src => src.MapFrom(m => m.ImdbId))
			   .ForMember(dest => dest.Title, src => src.MapFrom(m => m.Title))
			   .ForMember(
					dest => dest.Genres,
					src => src.MapFrom(m => m.Genres.Select(s => s.Name)))
			   .ForMember(dest => dest.TmdbId, src => src.MapFrom(m => m.Id));

			CreateMap<TvDetail, VideoDetail>()
			   .ForMember(dest => dest.Credits, src => src.Ignore())
			   .ForMember(dest => dest.ImdbId, src => src.MapFrom(m => m.ImdbId))
			   .ForMember(dest => dest.Title, src => src.MapFrom(m => m.Name))
			   .ForMember(dest => dest.TmdbId, src => src.MapFrom(m => m.Id));

			CreateMap<ProductionCompanyDetail, ProductionCompany>();
		}
	}
}