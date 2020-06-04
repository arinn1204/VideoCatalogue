using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Client.HostedServices.Models;
using Client.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Client.HostedServices.Services
{
	public class VideoQueueWorker : IHostedService
	{
		private readonly QueueInformationSettings _options;
		private readonly QueueClient _queueClient;
		private readonly IVideoRenamer _renamer;

		public VideoQueueWorker(
			IOptions<QueueInformationSettings> options,
			IVideoRenamer videoRenamer,
			QueueClient queueClient)
			=> (_options, _queueClient, _renamer) = (options.Value, queueClient, videoRenamer);

#region IHostedService Members

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			var timeout = _options.TimeoutValue;

			var response =
				await _queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

			if (response.Status != (int) HttpStatusCode.OK)
			{
				return;
			}

			while (await _queueClient.ExistsAsync(cancellationToken))
			{
				var responses = await _queueClient.ReceiveMessagesAsync(
					1,
					TimeSpan.FromMinutes(timeout),
					cancellationToken);

				if (responses.Value.Length <= 0)
				{
					return;
				}

				var messages = responses.Value;

				await _renamer.ProcessMessage(messages, cancellationToken);

				var deleteMessages = messages.Aggregate(
					Enumerable.Empty<Task>(),
					(current, message) => current.Append(
						_queueClient.DeleteMessageAsync(
							message.MessageId,
							message.PopReceipt,
							cancellationToken)));

				await Task.WhenAll(deleteMessages);
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

#endregion
	}
}