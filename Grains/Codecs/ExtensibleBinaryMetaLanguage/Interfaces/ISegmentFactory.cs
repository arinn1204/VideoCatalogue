namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces
{
	public interface ISegmentFactory
	{
		ISegmentChild GetChild(string name);
	}
}