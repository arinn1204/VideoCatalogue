using System;
using AutoMapper;
using Grains.Codecs.Models.Mappers;

namespace Grains.Tests.Unit.Fixtures
{
	public class MapperFixture : IDisposable
	{
		public MapperFixture()
		{
			MappingProfile = new Mapper(
				new MapperConfiguration(
					config => config.AddMaps(typeof(FileInformationProfile).Assembly)));
		}

		public IMapper MappingProfile { get; }

#region IDisposable Members

		public void Dispose()
		{
		}

#endregion
	}
}