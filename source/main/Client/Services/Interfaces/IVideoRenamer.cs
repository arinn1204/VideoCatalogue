using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues.Models;

namespace Client.Services.Interfaces
{
	public interface IVideoRenamer
	{
		Task ProcessMessage(
			IEnumerable<QueueMessage> message,
			CancellationToken cancellationToken);
	}
}