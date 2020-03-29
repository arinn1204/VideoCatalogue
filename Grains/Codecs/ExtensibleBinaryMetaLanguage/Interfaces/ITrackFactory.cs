namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces
{
	public interface ITrackFactory
	{
		ITrackReader GetTrackReader(string elementName);
	}
}