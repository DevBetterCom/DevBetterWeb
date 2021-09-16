using AutoMapper;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Vimeo.Models;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Models.Vimeo;

namespace DevBetterWeb.Web.MappingProfiles
{
  public class VideoProfile : Profile
  {
    public VideoProfile()
    {
      CreateMap<Video, VideoModel>().ReverseMap();
      CreateMap<ArchiveVideo, ArchiveVideoDto>().ReverseMap();
    }
  }
}
