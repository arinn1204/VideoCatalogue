using AutoMapper;
using GrainsInterfaces.Models.VideoApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grains.VideoApi.Models.VideoApi.Profiles
{
    public class VideoDetailProfile : Profile
    {
        public VideoDetailProfile()
        {
            CreateMap<MovieDetail, VideoDetail>()
                .ForMember(dest => dest.Credits, src => src.Ignore())
                .ForMember(dest => dest.ImdbId, src => src.MapFrom(m => m.ImdbId))
                .ForMember(dest => dest.Title, src => src.MapFrom(m => m.Title))
                .ForMember(dest => dest.TmdbId, src => src.MapFrom(m => m.Id));
        }
    }
}
