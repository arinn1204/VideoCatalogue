using System.Threading.Tasks;
using GrainsInterfaces.VideoApi.Models;

namespace GrainsInterfaces.VideoApi
{
	public interface IVideoApi
	{
		Task<VideoDetail> GetVideoDetails(VideoRequest request);
	}
}