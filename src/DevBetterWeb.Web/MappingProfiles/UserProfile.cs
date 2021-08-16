using AutoMapper;
using DevBetterWeb.Vimeo.Models;
using DevBetterWeb.Web.Models.Vimeo;

namespace DevBetterWeb.Web.MappingProfiles
{
  public class UserProfile : Profile
  {
    public UserProfile()
    {
      CreateMap<User, UserModel>();
    }
  }
}
