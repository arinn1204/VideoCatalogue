namespace GrainsInterfaces.VideoApi.Models
{
	public class ProductionCompany
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string LogoPath { get; set; } = string.Empty;
		public string OriginCountry { get; set; } = string.Empty;
	}
}