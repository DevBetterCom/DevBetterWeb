using AutoMapper;
using DevBetterWeb.Vimeo.Models;
using DevBetterWeb.Web.Models.Vimeo;

namespace DevBetterWeb.Web.MappingProfiles
{
  public class TagProfile : Profile
  {
    public TagProfile()
    {
      CreateMap<Tag, TagModel>();
    }
  }
}
