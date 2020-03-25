namespace Grains.Codecs.ExtensibleBinaryMetaLanguage
{
	public interface ISegmentFactory
	{
		ISegmentChild GetChild(string name);
	}
}