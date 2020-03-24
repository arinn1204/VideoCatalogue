namespace Grains.Codecs.Matroska.Models
{
	public class MatroskaError
	{
		public MatroskaError(string description)
		{
			Description = description;
		}

		public string Description { get; }
	}
}