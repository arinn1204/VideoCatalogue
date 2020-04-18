using System.Collections.Generic;
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
					src => src.MapFrom(m => m.Patterns.SelectMany(BuildRegex)));
		}

		private IEnumerable<Regex> BuildRegex(string pattern)
		{
			var parts = pattern.Split("&FILTER&");

			foreach (var part in parts)
			{
				yield return new Regex(part);
			}
		}
	}
}