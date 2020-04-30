﻿using AutoMapper;
using Grains.VideoInformation.Models.Credits;
using GrainsInterfaces.Models.VideoApi;

namespace Grains.VideoInformation.Models.Profiles
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