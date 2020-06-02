using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Grains.FileFormat.Interfaces;
using Grains.FileFormat.Models;

namespace Grains.FileFormat
{
	public class FileFormatRepository : IFileFormatRepository
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IMapper _mapper;

		public FileFormatRepository(
			IHttpClientFactory httpClientFactory,
			IMapper mapper)
		{
			_httpClientFactory = httpClientFactory;
			_mapper = mapper;
		}

#region IFileFormatRepository Members

		public IAsyncEnumerable<RegisteredFileFormat> GetAcceptableFileFormats()
		{
			var responseContentTask = GetResponseContent("fileFormats");

			return AsyncEnumerable.Create(
				token => EnumerateContent(
						responseContentTask,
						(FilePattern response)
							=> _mapper.Map<RegisteredFileFormat>(response))
				   .GetAsyncEnumerator(token));
		}

		public IAsyncEnumerable<string> GetAllowedFileTypes()
		{
			var responseContentTask = GetResponseContent("fileTypes");

			return AsyncEnumerable.Create(
				token => EnumerateContent(
						responseContentTask,
						(string response) => response)
				   .GetAsyncEnumerator(token));
		}

		public IAsyncEnumerable<string> GetFilteredKeywords()
		{
			var responseContentTask = GetResponseContent("filteredKeywords");

			return AsyncEnumerable.Create(
				token => EnumerateContent(
						responseContentTask,
						(string response) => response)
				   .GetAsyncEnumerator(token));
		}

		public async Task<string> GetTargetTitleFormat()
			=> await GetResponseContent("targetTitleFormat");

#endregion

		private async Task<string> GetResponseContent(string relativePath)
		{
			var client = _httpClientFactory.CreateClient(nameof(FileFormatRepository));
			var request = new HttpRequestMessage
			              {
				              Method = HttpMethod.Get,
				              RequestUri = new Uri(relativePath.TrimStart('/'), UriKind.Relative)
			              };

			var responseMessage = await client.SendAsync(request);
			var responseContent = await responseMessage
			                           .Content
			                           .ReadAsStringAsync()
				;
			return responseContent;
		}


		private async IAsyncEnumerable<TResult> EnumerateContent<TResponse, TResult>(
			Task<string> responseContent,
			Func<TResponse, TResult> getResult)
		{
			var content = await responseContent;
			var acceptableFormats =
				JsonSerializer.Deserialize<IEnumerable<TResponse>>(
					content,
					new JsonSerializerOptions
					{
						PropertyNamingPolicy = JsonNamingPolicy.CamelCase
					});

			foreach (var acceptableFormat in acceptableFormats)
			{
				yield return getResult(acceptableFormat);
			}
		}
	}
}