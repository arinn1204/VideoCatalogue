namespace Grains.FileFormat.Models
{
	public enum TorrentStatus
	{
		Stopped,
		Queued,
		CheckingFiles,
		Downloading,
		Seeding
	}
}