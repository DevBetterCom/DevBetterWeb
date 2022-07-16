using AutoMapper;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Web.Models;

namespace DevBetterWeb.Web.MappingProfiles;

public class QuestionProfile : Profile
{
  public QuestionProfile()
  {
    CreateMap<Question, QuestionDto>()
	    .ForPath(dest => dest.MemberName,
		    opt => opt.MapFrom(source => source.MemberWhoCreate!.UserFullName()));
	}
}
