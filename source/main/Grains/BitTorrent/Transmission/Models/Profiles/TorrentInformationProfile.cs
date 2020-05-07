using System.Linq;
using AutoMapper;
using Grains.FileFormat.Models;

namespace Grains.BitTorrent.Transmission.Models.Profiles
{
	public class TorrentInformationProfile : Profile
	{
		public TorrentInformationProfile()
		{
			CreateMap<TorrentResponse, TorrentInformation>()
			   .ForMember(
					dest => dest.Name,
					src => src.MapFrom(m => m.Name))
			   .ForMember(
					dest => dest.CompletedFileNames,
					src => src.MapFrom(m => m.Files.Where(w => w.IsComplete).Select(s => s.Name)))
			   .ForMember(
					dest => dest.Status,
					src => src.MapFrom(m => GetTorrentStatus(m.Status)))
			   .ForMember(
					dest => dest.QueuedStatus,
					src => src.MapFrom(m => GetQueuedStatus(m.Status)));
		}

		private TorrentStatus GetTorrentStatus(int status)
			=> status switch
			   {
				   0 => TorrentStatus.Stopped,
				   2 => TorrentStatus.CheckingFiles,
				   4 => TorrentStatus.Downloading,
				   6 => TorrentStatus.Seeding,
				   _ => TorrentStatus.Queued
			   };


		private QueuedStatus? GetQueuedStatus(int status)
			=> status switch
			   {
				   1 => QueuedStatus.CheckFiles,
				   3 => QueuedStatus.Download,
				   5 => QueuedStatus.Seed,
				   _ => null
			   };
	}
}