using System.Text.Json.Serialization;

namespace Grains.VideoInformation.Models.Details
{
	public class ProductionCompanyDetail
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;

		[JsonPropertyName("logo_path")]
		public string LogoPath { get; set; } = string.Empty;

		[JsonPropertyName("origin_country")]
		public string OriginCountry { get; set; } = string.Empty;
	}
}