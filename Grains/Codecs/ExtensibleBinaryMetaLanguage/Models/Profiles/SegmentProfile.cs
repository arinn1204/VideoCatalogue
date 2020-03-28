using AutoMapper;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Profiles
{
	public class SegmentProfile : Profile
	{
		public SegmentProfile()
		{
			CreateMap<Segment, Segment>();
		}
	}
}