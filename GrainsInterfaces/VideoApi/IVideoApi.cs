using System.Threading.Tasks;
using GrainsInterfaces.Models.VideoApi;

namespace GrainsInterfaces.VideoApi
{
	public interface IVideoApi
	{
		Task<VideoDetail> GetVideoDetails(VideoRequest request);
	}
}