using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Grains.VideoSearcher.Interfaces;
using Grains.VideoSearcher.Models;
using Grains.VideoSearcher.Repositories.Models;
using Newtonsoft.Json;

namespace Grains.VideoSearcher.Repositories
{
	public class FileFormatRepository2 : IFileFormatRepository
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IMapper _mapper;

		public FileFormatRepository2(
			IHttpClientFactory httpClientFactory,
			IMapper mapper)
		{
			_httpClientFactory = httpClientFactory;
			_mapper = mapper;
		}

#region IFileFormatRepository Members

		public async IAsyncEnumerable<FileFormat> GetAcceptableFileFormats()
		{
			var responseContent = await GetResponseContent("fileFormats");
			var acceptableFormats =
				JsonConvert.DeserializeObject<IEnumerable<FilePattern>>(responseContent);

			foreach (var acceptableFormat in acceptableFormats)
			{
				yield return _mapper.Map<FileFormat>(acceptableFormat);
			}
		}

		public async IAsyncEnumerable<string> GetAllowedFileTypes()
		{
			var responseContent = await GetResponseContent("fileTypes");
			var fileTypes = JsonConvert.DeserializeObject<IEnumerable<string>>(responseContent);

			foreach (var fileType in fileTypes)
			{
				yield return fileType;
			}
		}

		public async IAsyncEnumerable<string> GetFilteredKeywords()
		{
			var responseContent = await GetResponseContent("filteredKeywords");
			var filteredKeywords =
				JsonConvert.DeserializeObject<IEnumerable<string>>(responseContent);

			foreach (var filteredKeyword in filteredKeywords)
			{
				yield return filteredKeyword;
			}
		}

#endregion

		private async Task<string> GetResponseContent(string relativePath)
		{
			var client = _httpClientFactory.CreateClient(nameof(FileFormatRepository2));
			var request = new HttpRequestMessage
			              {
				              Method = HttpMethod.Get,
				              RequestUri = new Uri(relativePath.TrimStart('/'), UriKind.Relative)
			              };

			var responseMessage = await client.SendAsync(request);
			var responseContent = await responseMessage.Content.ReadAsStringAsync();
			return responseContent;
		}
	}
}