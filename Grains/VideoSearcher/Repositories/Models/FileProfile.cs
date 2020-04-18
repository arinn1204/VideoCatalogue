using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using Grains.VideoSearcher.Models;

namespace Grains.VideoSearcher.Repositories.Models
{
	public class FileProfile : Profile
	{
		public FileProfile()
		{
			CreateMap<FilePattern, FileFormat>()
			   .ForMember(
					dest => dest.Patterns,
					src => src.MapFrom(m => m.Patterns.Select(s => new Regex(s))));
		}
	}
}