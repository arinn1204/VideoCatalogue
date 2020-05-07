using System.Threading.Tasks;
using GrainsInterfaces.VideoApi.Models;
using Orleans;

namespace GrainsInterfaces.VideoApi
{
	public interface IVideoApi : IGrainWithGuidKey
	{
		Task<VideoDetail> GetVideoDetails(VideoRequest request);
	}
}