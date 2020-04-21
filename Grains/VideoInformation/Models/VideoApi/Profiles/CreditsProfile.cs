using AutoMapper;
using Grains.VideoInformation.Models.VideoApi.Credits;
using GrainsInterfaces.Models.VideoApi;

namespace Grains.VideoInformation.Models.VideoApi.Profiles
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