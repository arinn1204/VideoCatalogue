using System.Text.Json.Serialization;

namespace Grains.VideoInformation.Models.Credits
{
	public class PersonCredit
	{
		public int Gender { get; set; }
		public string Name { get; set; } = string.Empty;

		[JsonPropertyName("profile_path")]
		public string ProfilePath { get; set; } = string.Empty;

		[JsonPropertyName("cast_id")]
		public int CastId { get; set; }
	}
}