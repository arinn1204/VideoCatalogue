using AutoMapper;
using Grains.Codecs.Matroska.Models;
using GrainsInterfaces.Models.CodecParser;

namespace Grains.Codecs.Models.Mappers
{
	public class FileErrorProfile : Profile
	{
		public FileErrorProfile()
		{
			CreateMap<FileError, MatroskaError>();
		}
	}
}