namespace Grains.BitTorrent.Transmission.Models
{
	public class FileResponse
	{
		public ulong BytesCompleted { get; set; }
		public ulong Length { get; set; }

		public string Name { get; set; }
			= string.Empty;

		public bool IsComplete => BytesCompleted == Length;
	}
}