using AutoMapper;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Web.Models;

namespace DevBetterWeb.Web.MappingProfiles;

public class VideoCommentProfile : Profile
{
	public VideoCommentProfile()
	{
		CreateMap<VideoComment, VideoCommentDto>()
			.ForPath(dest => dest.MemberName,
				opt => opt.MapFrom(source => source.MemberWhoCreate!.FirstName));
		CreateMap<VideoCommentDto, VideoComment>();
	}
}
