using AutoMapper;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment;

namespace Grains.Tests.Unit.TestUtilities
{
	public static class MapperHelper
	{
		public static IMapper CreateMapper()
		{
			return new Mapper(
				new MapperConfiguration(
					cfg =>
					{
						cfg.AddMaps(typeof(Segment).Assembly);
					}));
		}
	}
}