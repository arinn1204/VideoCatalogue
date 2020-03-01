using AutoMapper;
using GrainsInterfaces.Models.VideoApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grains.VideoApi.Models.VideoApi.Profiles
{
    public class CreditsProfile : Profile
    {
        public CreditsProfile()
        {

            CreateMap<MovieCredit, Credit>()
                .ForMember(dest => dest.Cast, src => src.MapFrom(m => m.Cast))
                .ForMember(dest => dest.Crew, src => src.MapFrom(m => m.Crew));

            CreateMap<CrewCredit, Crew>();
            CreateMap<CastCredit, Cast>();

            CreateMap<CastCredit, GuestStar>();
        }
    }
}
