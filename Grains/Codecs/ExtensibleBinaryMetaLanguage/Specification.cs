using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Grains.Codecs.Matroska.Interfaces;
using Grains.Codecs.Matroska.Models;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models
{
	public class Specification
		: ISpecification
	{
		private const string ClientRegistrationName = "MatroskaClient";
		private readonly IHttpClientFactory _httpClientFactory;

		public Specification(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

#region ISpecification Members

		public async Task<EbmlSpecification> GetSpecification()
		{
			var responseMessage = await GetContent();
			var responseContent = await responseMessage.Content.ReadAsStreamAsync();
			var content = SerializeFromXml(responseContent);
			return content;
		}

#endregion

		private EbmlSpecification SerializeFromXml(Stream content)
		{
			var serializer = new XmlSerializer(typeof(EbmlSpecification));
			return (EbmlSpecification) serializer.Deserialize(content);
		}

		private async Task<HttpResponseMessage> GetContent()
		{
			var client = _httpClientFactory.CreateClient(ClientRegistrationName);

			var requestMessage = new HttpRequestMessage
			                     {
				                     RequestUri = client.BaseAddress,
				                     Method = HttpMethod.Get
			                     };

			var response = await client.SendAsync(requestMessage);

			_ = response.IsSuccessStatusCode || HandleResponse(response);

			return response;
		}

		private bool HandleResponse(HttpResponseMessage response)
			=> throw new MatroskaException("Cannot retrieve specification from the source.");
	}
}