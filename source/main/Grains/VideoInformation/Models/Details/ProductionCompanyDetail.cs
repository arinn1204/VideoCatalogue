using Newtonsoft.Json;

namespace Grains.VideoInformation.Models.Details
{
	[JsonObject]
	public class ProductionCompanyDetail
	{
		[JsonProperty]
		public int Id { get; set; }

		[JsonProperty]
		public string Name { get; set; } = string.Empty;

		[JsonProperty("logo_path")]
		public string LogoPath { get; set; } = string.Empty;

		[JsonProperty("origin_country")]
		public string OriginCountry { get; set; } = string.Empty;
	}
}