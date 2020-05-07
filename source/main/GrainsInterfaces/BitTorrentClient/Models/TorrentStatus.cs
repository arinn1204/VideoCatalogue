namespace GrainsInterfaces.BitTorrentClient.Models
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