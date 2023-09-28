using AutoMapper;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Models.Vimeo;
using NimblePros.Vimeo.Models;

namespace DevBetterWeb.Web.MappingProfiles;

public class VideoProfile : Profile
{
	public VideoProfile()
	{
		CreateMap<Video, VideoModel>().ReverseMap();
		CreateMap<ArchiveVideo, ArchiveVideoDto>().ReverseMap();
	}
}
