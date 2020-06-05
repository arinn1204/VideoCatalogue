using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Client.HostedServices.Models;
using Client.HostedServices.Services;
using Client.Services.Interfaces;
using Client.Tests.Unit.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Client.Tests.Unit.HostedServices
{
	public class VideoQueueWorkerTests : IClassFixture<ConfigurationFixture>
	{
#region Setup/Teardown

		public VideoQueueWorkerTests(ConfigurationFixture fixture)
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
			_fixture.Register<IHostedService>(() => _fixture.Create<VideoQueueWorker>());
			var options = Options.Create(
				new QueueInformationSettings
				{
					QueueName = GetType().Name,
					TimeoutValue = 300
				});

			_fixture.Inject(options);
			_fixture.Register(() => _fixture.Create<Mock<QueueClient>>().Object);
			_fixture.Freeze<Mock<IVideoRenamer>>()
			        .Setup(
				         s => s.ProcessMessage(
					         It.IsAny<IEnumerable<QueueMessage>>(),
					         _cancellationToken))
			        .ReturnsAsync(
				         new[]
				         {
					         new RenamerResult
					         {
						         Result = Result.Success,
						         MessageId = "Message",
						         PopReceipt = "Pop"
					         }
				         });
			_cancellationToken = new CancellationToken();

			var response = new Mock<Response>();
			response.Setup(s => s.Status)
			        .Returns((int) HttpStatusCode.OK);
			var queue = _fixture.Freeze<Mock<QueueClient>>();
			queue.Setup(s => s.CreateIfNotExistsAsync(null, _cancellationToken))
			     .ReturnsAsync(response.Object);

			queue.SetupSequence(s => s.ExistsAsync(_cancellationToken))
			     .ReturnsAsync(
				      () =>
				      {
					      var valueResponse = new Mock<Response<bool>>();
					      valueResponse.Setup(s => s.Value)
					                   .Returns(true);
					      return valueResponse.Object;
				      })
			     .ReturnsAsync(
				      () =>
				      {
					      var valueResponse = new Mock<Response<bool>>();
					      valueResponse.Setup(s => s.Value)
					                   .Returns(false);
					      return valueResponse.Object;
				      });
			queue.Setup(s => s.DeleteMessageAsync("Message", "Pop", _cancellationToken))
			     .ReturnsAsync(
				      () =>
				      {
					      var response = new Mock<Response>();
					      return response.Object;
				      });
			queue.Setup(
				      s => s.ReceiveMessagesAsync(
					      It.IsAny<int?>(),
					      It.IsAny<TimeSpan?>(),
					      _cancellationToken))
			     .Returns(
				      () =>
				      {
					      var message = new Mock<QueueMessage>();
					      var response = new Mock<Response<QueueMessage[]>>();
					      response.Setup(s => s.Value)
					              .Returns(
						               new[]
						               {
							               message.Object
						               });
					      return Task.FromResult(response.Object);
				      });
		}

#endregion

		private readonly Fixture _fixture;
		private readonly CancellationToken _cancellationToken;

		[Fact]
		public async Task ShouldContinueIfCreateReturnsNull()
		{
			var response = new Mock<Response>();
			response.Setup(s => s.Status)
			        .Returns((int) HttpStatusCode.NotFound);
			var queue = _fixture.Freeze<Mock<QueueClient>>();
			queue.Setup(s => s.CreateIfNotExistsAsync(null, _cancellationToken))
			     .ReturnsAsync(null as Response);

			await _fixture.Create<IHostedService>().StartAsync(_cancellationToken);

			queue.Verify(v => v.ExistsAsync(_cancellationToken), Times.Exactly(2));
		}

		[Fact]
		public async Task ShouldNotAttemptToCheckIfQueueExistsIfCreateFails()
		{
			var response = new Mock<Response>();
			response.Setup(s => s.Status)
			        .Returns((int) HttpStatusCode.NotFound);
			var queue = _fixture.Freeze<Mock<QueueClient>>();
			queue.Setup(s => s.CreateIfNotExistsAsync(null, _cancellationToken))
			     .ReturnsAsync(response.Object);

			await _fixture.Create<IHostedService>().StartAsync(_cancellationToken);

			queue.Verify(v => v.ExistsAsync(_cancellationToken), Times.Never);
		}

		[Fact]
		public async Task ShouldNotAttemptToProcessMessagesIfNoneAreReturned()
		{
			var renamer = _fixture.Freeze<Mock<IVideoRenamer>>();
			var queue = _fixture.Freeze<Mock<QueueClient>>();
			queue.Setup(
				      s => s.ReceiveMessagesAsync(
					      It.IsAny<int?>(),
					      It.IsAny<TimeSpan?>(),
					      _cancellationToken))
			     .Returns<int?, TimeSpan?, CancellationToken>(
				      (numberOfMessages, visibilityTimeout, token) =>
				      {
					      var response = new Mock<Response<QueueMessage[]>>();
					      response.Setup(s => s.Value)
					              .Returns(
						               new QueueMessage[]
						               {
						               });
					      return Task.FromResult(response.Object);
				      });

			await _fixture.Create<IHostedService>().StartAsync(_cancellationToken);

			renamer.Verify(
				v => v.ProcessMessage(It.IsAny<IEnumerable<QueueMessage>>(), _cancellationToken),
				Times.Never);
		}

		[Fact]
		public async Task ShouldPutTimeoutDefinedInConfigurationWhenReadingMessages()
		{
			var queue = _fixture.Freeze<Mock<QueueClient>>();
			var timeout = new TimeSpan();
			var messages = -1;

			queue.Setup(
				      s => s.ReceiveMessagesAsync(
					      It.IsAny<int?>(),
					      It.IsAny<TimeSpan?>(),
					      _cancellationToken))
			     .Returns<int?, TimeSpan?, CancellationToken>(
				      (numberOfMessages, visibilityTimeout, token) =>
				      {
					      timeout = visibilityTimeout!.Value;
					      messages = numberOfMessages!.Value;

					      var message = new Mock<QueueMessage>();

					      var response = new Mock<Response<QueueMessage[]>>();
					      response.Setup(s => s.Value)
					              .Returns(
						               new[]
						               {
							               message.Object
						               });
					      return Task.FromResult(response.Object);
				      });

			await _fixture.Create<IHostedService>().StartAsync(_cancellationToken);

			timeout.Should()
			       .Be(TimeSpan.FromMinutes(300));

			messages.Should()
			        .Be(1);
		}
	}
}