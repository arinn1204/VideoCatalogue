﻿using System.Threading.Tasks;
using GrainsInterfaces.Models.VideoApi;
using Orleans;

namespace GrainsInterfaces.VideoApi
{
	public interface IVideoApi : IGrainWithGuidKey
	{
		Task<VideoDetail> GetVideoDetails(VideoRequest request);
	}
}