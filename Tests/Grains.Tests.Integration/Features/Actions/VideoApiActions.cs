using BoDi;
using GrainsInterfaces.Models.VideoApi;
using GrainsInterfaces.VideoApi;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Actions
{
    [Binding]
    public class VideoApiActions
    {
        private readonly IVideoApi _videoApi;
        private readonly VideoRequest _request;
        private readonly IObjectContainer _container;

        public VideoApiActions(
            IVideoApi videoApi,
            VideoRequest request,
            IObjectContainer container)
        {
            _videoApi = videoApi;
            _request = request;
            _container = container;
        }

        [When(@"the client requests movie details")]
        public async Task WhenTheClientRequestsMovieDetails()
        {
            var details = await _videoApi.GetVideoDetails(_request);
            _container.RegisterInstanceAs(details);
        }

    }
}
