namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models
{
	public class SeekHead
	{
		/// <summary>
		///     Gets or sets the binary ID corresponding to the SeekPosition.
		/// </summary>
		public long SeekId { get; set; }

		/// <summary>
		///     The position that the element is located at.
		/// </summary>
		public uint SeekPosition { get; set; }

		/// <summary>
		///     The element that the position describes.
		/// </summary>
		public EbmlElement Element { get; set; }
	}
}