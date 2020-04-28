using System.Text.Json.Serialization;

namespace Grains.BitTorrent.Transmission.Models
{
	public class TransmissionRequest
	{
		[JsonPropertyName("arguments")]
		public RpcParameters RpcParameters { get; set; }
			= new RpcParameters();

		public string Method { get; set; }
			= RpcMethod.None.ToString();

		public int? Tag { get; set; }
	}
}