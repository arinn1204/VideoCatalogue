using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;

namespace Grains.FileFormat.Models
{
	public class FileProfile : Profile
	{
		public FileProfile()
		{
			CreateMap<FilePattern, RegisteredFileFormat>()
			   .ForMember(
					dest => dest.CapturePattern,
					src => src.MapFrom(s => s.Pattern));

			CreateMap<Pattern, CapturePattern>()
			   .ForMember(
					dest => dest.Capture,
					src => src.MapFrom(m => new Regex(m.Capture)))
			   .ForMember(
					dest => dest.NegativeFilters,
					src => src.MapFrom(m => m.NegativeFilters.Select(s => new Regex(s))))
			   .ForMember(
					dest => dest.PositiveFilters,
					src => src.MapFrom(m => m.PositiveFilters.Select(s => new Regex(s))));
		}
	}
}