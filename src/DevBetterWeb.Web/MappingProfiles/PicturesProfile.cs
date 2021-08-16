using AutoMapper;
using DevBetterWeb.Vimeo.Models;
using DevBetterWeb.Web.Models.Vimeo;

namespace DevBetterWeb.Web.MappingProfiles
{
  public class PicturesProfile : Profile
  {
    public PicturesProfile()
    {
      CreateMap<Pictures, PicturesModel>();
    }
  }
}
