using FluentAssertions;
using GrainsInterfaces.Models.VideoApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Text;
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
            var baseFileName = GetFilename(title);
            var fileEncoding = Encoding.UTF8;

            var (imdbId, tmdbId) = getIds(baseFileName, fileEncoding);
            var credits = GetCredits(baseFileName, fileEncoding);

            _details.Should()
                .BeEquivalentTo(new VideoDetail
                {
                    Title = title,
                    ImdbId = imdbId,
                    TmdbId = tmdbId,
                    Credits = credits
                });
        }

        private Credit GetCredits(string baseFileName, Encoding encoding)
        {
            var filename = $"{BuildFilePath(baseFileName)}.credits.json";
            var fileBytes = File.ReadAllBytes(filename);
            var fileContents = encoding.GetString(fileBytes);
            return
                JsonConvert.DeserializeObject<Credit>(
                    fileContents,
                    new JsonSerializerSettings
                {
                    ContractResolver = new CreditResolver()
                });
        }

        private (string imdbId, int tmdbId) getIds(string baseFileName, Encoding encoding)
        {
            var filename = $"{BuildFilePath(baseFileName)}.json";
            var fileData = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(filename, encoding));

            return int.TryParse(fileData["id"].ToObject<string>(), out var tmdbId)
                ? (fileData["imdb_id"].ToObject<string>(), tmdbId)
                : (fileData["imdb_id"].ToObject<string>(), -1);
        }

        private string BuildFilePath(string baseFileName)
        {
            return Path.Combine("TestData", "VideoApi", baseFileName);
        }

        private string GetFilename(string title)
        {
            return string.Join(string.Empty, title.Split(' ')
                                                  .Select((word, index) =>
                                                    {
                                                        if (index == 0)
                                                        {
                                                            return word.ToLower();
                                                        }
                                                        else
                                                        {
                                                            var firstCharacter = char.ToUpper(word.First());
                                                            var restOfWord = string.Join(string.Empty, word.Skip(1)).ToLower();
                                                            return $"{firstCharacter}{restOfWord}";
                                                        }
                                                    }));
        }
    }
}
