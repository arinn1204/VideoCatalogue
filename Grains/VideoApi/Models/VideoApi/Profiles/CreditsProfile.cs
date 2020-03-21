using AutoMapper;
using GrainsInterfaces.Models.VideoApi;

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