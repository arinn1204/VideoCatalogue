using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Grains.Tests.Unit.TestUtilities
{
	public static class MockHttpClient
	{
		public static Func<(HttpClient client, HttpRequestMessage request, int callCounter)>
			GetFakeHttpClient(
				string response = "{}",
				string contentType = "application/json",
				string baseAddress = "http://localhost",
				HttpStatusCode statusCode = HttpStatusCode.OK,
				Func<int, HttpResponseMessage> customResponse = null)
		{
			var content = new StringContent(response, Encoding.UTF8, contentType);

			var handler =
				customResponse == null
					? new MockHandler(content, statusCode)
					: new MockHandler(customResponse);

			return () =>
			       {
				       var client = new HttpClient(handler);

				       if (!string.IsNullOrWhiteSpace(baseAddress))
				       {
					       client.BaseAddress = new Uri(baseAddress);
				       }

				       return (client, handler.Request, handler.CallCounter);
			       };
		}

#region Nested type: MockHandler

		private class MockHandler : HttpMessageHandler
		{
			private readonly Func<int, HttpResponseMessage>
				_createResponse;

			public HttpRequestMessage Request
				= new HttpRequestMessage();

			public MockHandler(HttpContent responseContent, HttpStatusCode statusCode)
			{
				_createResponse = _ => new HttpResponseMessage
				                       {
					                       Content = responseContent,
					                       StatusCode = statusCode
				                       };
			}

			public MockHandler(Func<int, HttpResponseMessage> createResponse)
			{
				_createResponse = createResponse;
			}

			public int CallCounter { get; private set; }

			protected override Task<HttpResponseMessage> SendAsync(
				HttpRequestMessage request,
				CancellationToken cancellationToken)
			{
				Request = request;
				CallCounter++;
				return Task.FromResult(_createResponse(CallCounter));
			}
		}

#endregion
	}
}