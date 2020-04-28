namespace Grains.BitTorrent.Transmission.Models
{
	public class RpcMethod
	{
		private readonly string _method;

		private RpcMethod(string method)
		{
			_method = method;
		}

		public static RpcMethod Get
			=> new RpcMethod("torrent-get");

		public static RpcMethod None
			=> new RpcMethod("NONE");

		public override string ToString() => _method;
	}
}