using Newtonsoft.Json;

namespace Grains.VideoInformation.Models.Details
{
	[JsonObject]
	public class ProductionCompanyDetail
	{
		[JsonProperty]
		public int Id { get; set; }

		[JsonProperty]
		public string Name { get; set; }

		[JsonProperty("logo_path")]
		public string LogoPath { get; set; }

		[JsonProperty("origin_country")]
		public string OriginCountry { get; set; }
	}
}