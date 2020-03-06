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

            CreateMap<MovieCredit, Credit>();
            CreateMap<TvCredit, Credit>();
            CreateMap<CrewCredit, Crew>();
            CreateMap<CastCredit, Cast>();
            CreateMap<CastCredit, GuestStar>();
        }
    }
}
