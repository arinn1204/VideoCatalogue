namespace GrainsInterfaces.BitTorrent.Models
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