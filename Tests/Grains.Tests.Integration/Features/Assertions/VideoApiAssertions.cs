using System.IO;
using System.Text;
using FluentAssertions;
using Grains.Tests.Integration.Extensions;
using Grains.Tests.Integration.Features.Models.Resolvers;
using GrainsInterfaces.Models.VideoApi;
using Newtonsoft.Json;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Assertions
{
	[Binding]
	public class VideoApiAssertions
	{
		private readonly VideoDetail _details;

		public VideoApiAssertions(VideoDetail details)
		{
			_details = details;
		}

		[Then(@"the client is given the information about (.*)")]
		public void ThenTheClientIsGivenTheInformation(string title)
		{
			var baseFileName = title.ToFileBaseName();
			var fileEncoding = Encoding.UTF8;

			var videoDetails = GetDetails(baseFileName, fileEncoding);
			var credits = GetCredits(baseFileName, fileEncoding);

			_details.Should()
			        .BeEquivalentTo(
				         new VideoDetail
				         {
					         Title = title,
					         ImdbId = videoDetails.ImdbId,
					         TmdbId = videoDetails.TmdbId,
					         Credits = credits,
					         Genres = videoDetails.Genres,
					         Overview = videoDetails.Overview,
					         ProductionCompanies = videoDetails.ProductionCompanies,
					         ReleaseDate = videoDetails.ReleaseDate,
					         Runtime = videoDetails.Runtime
				         });
		}

		private Credit GetCredits(string baseFileName, Encoding encoding)
		{
			var filename = $"{baseFileName.ToFilePath()}.credits.json";
			var fileContents = File.ReadAllText(filename, encoding);
			return
				JsonConvert.DeserializeObject<Credit>(
					fileContents,
					new JsonSerializerSettings
					{
						ContractResolver = new CreditResolver()
					});
		}

		private VideoDetail GetDetails(string baseFileName, Encoding encoding)
		{
			var filename = $"{baseFileName.ToFilePath()}.json";
			var fileContents = File.ReadAllText(filename, encoding);
			var fileData = JsonConvert.DeserializeObject<VideoDetail>(
				fileContents,
				new JsonSerializerSettings
				{
					ContractResolver = new VideoDetailResolver()
				});

			return fileData;
		}
	}
}