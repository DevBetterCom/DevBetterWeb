using System.Linq;
using AutoMapper;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Web.Models;

namespace DevBetterWeb.Web.MappingProfiles;

public class MemberProfile : Profile
{
  public MemberProfile()
  {
		CreateMap<Member, MemberForBookDto>()
			.ForPath(dest => dest.FullName,
				opt => opt.MapFrom(source => source.UserFullName()));
		CreateMap<Member, MemberLinksDTO>();
  }
}
