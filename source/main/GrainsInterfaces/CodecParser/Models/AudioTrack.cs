#nullable enable
namespace GrainsInterfaces.CodecParser.Models
{
	public class AudioTrack
	{
		public Codec Codec { get; set; }
		public string? Language { get; set; }
		public double Frequency { get; set; }
		public int Channels { get; set; }
		public string? Name { get; set; }
	}
}