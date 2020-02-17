using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Grains.Tests.Unit.TestUtilities
{
    public static class MockHttpClient
    {
        private class MockHandler : HttpMessageHandler
        {
            private readonly HttpContent _responseContent;
            private readonly HttpStatusCode _statusCode;

            public MockHandler(HttpContent responseContent, HttpStatusCode statusCode = HttpStatusCode.OK)
            {
                _responseContent = responseContent;
                _statusCode = statusCode;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new HttpResponseMessage()
                {
                    Content = _responseContent,
                    StatusCode = _statusCode
                });
            }
        }

        public static HttpClient GetFakeHttpClient(string response)
        {
            var content = new StringContent(response, Encoding.UTF8, "application/json");

            var handler = new MockHandler(content);

            return new HttpClient(handler);
        }
    }
}
