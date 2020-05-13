namespace Grains.FileFormat.Models
{
	public class RegisteredFileFormat
	{
		public CapturePattern CapturePattern { get; set; }
			= new CapturePattern();

		public int TitleGroup { get; set; }
		public int? YearGroup { get; set; }
		public int? SeasonGroup { get; set; }
		public int? EpisodeGroup { get; set; }
		public int ContainerGroup { get; set; }
	}
}