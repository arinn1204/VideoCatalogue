using System.Text.Json.Serialization;

namespace Grains.BitTorrent.Transmission.Models
{
	public class TransmissionResponse
	{
		[JsonPropertyName("arguments")]
		public ResponseArguments ResponseArguments { get; set; }
			= new ResponseArguments();
	}
}