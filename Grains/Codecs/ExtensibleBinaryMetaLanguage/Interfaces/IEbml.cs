﻿using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.Matroska.Models;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces
{
	public interface IEbml
	{
		EbmlHeader GetHeaderInformation(
			Stream stream,
			EbmlSpecification ebmlSpecification);

		uint GetMasterIds(Stream stream, EbmlSpecification specification);
	}
}