using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using GrainsInterfaces.FileFormat.Models;

namespace Grains.FileFormat.Models
{
	public class FileProfile : Profile
	{
		public FileProfile()
		{
			CreateMap<FilePattern, RegisteredFileFormat>()
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