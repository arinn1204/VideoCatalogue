using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues.Models;

namespace Client.Services.Interfaces
{
	public interface IVideoRenamer
	{
		Task<IEnumerable<RenamerResult>> ProcessMessage(
			IEnumerable<QueueMessage> message,
			CancellationToken cancellationToken);
	}

	public class RenamerResult
	{
		public Result Result { get; set; }
		public string MessageId { get; set; }
		public string PopReceipt { get; set; }
	}

	public enum Result
	{
		Fail,
		Success
	}
}