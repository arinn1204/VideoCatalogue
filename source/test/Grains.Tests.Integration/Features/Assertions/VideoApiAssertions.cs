using System.IO;
using System.Text;
using System.Text.Json;
using AutoMapper;
using FluentAssertions;
using Grains.Tests.Integration.Extensions;
using Grains.VideoInformation.Models.Credits;
using Grains.VideoInformation.Models.Details;
using GrainsInterfaces.VideoApi.Models;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Assertions
{
	[Binding]
	public class VideoApiAssertions
	{
		private readonly VideoDetail _details;
		private readonly IMapper _mapper;

		public VideoApiAssertions(
			VideoDetail details,
			IMapper mapper)
		{
			_details = details;
			_mapper = mapper;
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
			var detail =
				JsonSerializer.Deserialize<MovieCredit>(
					fileContents,
					new JsonSerializerOptions
					{
						PropertyNamingPolicy = JsonNamingPolicy.CamelCase
					});

			return _mapper.Map<Credit>(detail);
		}

		private VideoDetail GetDetails(string baseFileName, Encoding encoding)
		{
			var filename = $"{baseFileName.ToFilePath()}.json";
			var fileContents = File.ReadAllText(filename, encoding);
			var movieDetail = JsonSerializer.Deserialize<MovieDetail>(
				fileContents,
				new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				});

			return _mapper.Map<VideoDetail>(movieDetail);
		}
	}
}