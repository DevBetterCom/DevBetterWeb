using AutoMapper;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Web.Models;

namespace DevBetterWeb.Web.MappingProfiles;

public class InvitationProfile : Profile
{
  public InvitationProfile()
  {
    CreateMap<Invitation, InvitationDto>().ReverseMap();
  }
}
