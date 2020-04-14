namespace GrainsInterfaces.Models.CodecParser
{
	public class AudioTrack
	{
		public Codec Codec { get; set; }
		public string Language { get; set; }
		public double Frequency { get; set; }
		public int Channels { get; set; }
	}
}