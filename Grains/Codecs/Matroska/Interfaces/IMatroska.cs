using System.IO;
using Grains.Codecs.Matroska.Models;

namespace Grains.Codecs.Matroska.Interfaces
{
	public interface IMatroska
	{
		bool IsMatroska(Stream stream);
		MatroskaData GetFileInformation(Stream stream, out MatroskaError error);
	}
}