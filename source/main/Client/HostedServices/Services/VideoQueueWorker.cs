using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Queues;
using Client.HostedServices.Models;
using Client.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Client.HostedServices.Services
{
	public class VideoQueueWorker : BackgroundService
	{
		private readonly QueueInformationSettings _options;
		private readonly QueueClient _queueClient;
		private readonly IVideoRenamer _renamer;

		public VideoQueueWorker(
			IOptions<QueueInformationSettings> options,
			IVideoRenamer videoRenamer,
			QueueClient queueClient)
			=> (_options, _queueClient, _renamer) = (options.Value, queueClient, videoRenamer);

		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			var response =
				await _queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

			if (response?.Status >= 400)
			{
				return;
			}

			while (await _queueClient.ExistsAsync(cancellationToken))
			{
				var responses = await _queueClient.ReceiveMessagesAsync(
					1,
					TimeSpan.FromMinutes(_options.TimeoutValue),
					cancellationToken);

				if (responses.Value.Length <= 0)
				{
					continue;
				}

				var results = await _renamer.ProcessMessage(responses.Value, cancellationToken);
				var deleteMessages =
					results.Where(w => w.Result == Result.Success)
					       .Aggregate(
						        Enumerable.Empty<Task<Response>>(),
						        (current, result) => current.Append(
							        _queueClient.DeleteMessageAsync(
								        result.MessageId,
								        result.PopReceipt,
								        cancellationToken)));

				await Task.WhenAll(deleteMessages);
			}
		}
	}
}