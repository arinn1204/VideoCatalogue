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
				string response,
				string contentType = "application/json",
				string baseAddress = "",
				HttpStatusCode statusCode = HttpStatusCode.OK)
		{
			var content = new StringContent(response, Encoding.UTF8, contentType);

			var handler = new MockHandler(content, statusCode);

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
			private readonly HttpContent _responseContent;
			private readonly HttpStatusCode _statusCode;

			public HttpRequestMessage Request;

			public MockHandler(HttpContent responseContent, HttpStatusCode statusCode)
			{
				_responseContent = responseContent;
				_statusCode = statusCode;
				Request = new HttpRequestMessage();
			}

			public int CallCounter { get; private set; }

			protected override Task<HttpResponseMessage> SendAsync(
				HttpRequestMessage request,
				CancellationToken cancellationToken)
			{
				Request = request;
				CallCounter++;
				return Task.FromResult(
					new HttpResponseMessage
					{
						Content = _responseContent,
						StatusCode = _statusCode
					});
			}
		}

#endregion
	}
}