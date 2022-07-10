using AutoMapper;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Web.Models;

namespace DevBetterWeb.Web.MappingProfiles;

public class VideoCommentProfile : Profile
{
  public VideoCommentProfile()
  {
	  CreateMap<VideoComment, VideoCommentDto>();
		CreateMap<VideoCommentDto, VideoComment>();
	}
}
