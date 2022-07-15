using System;
using AutoMapper;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Web.Models;

namespace DevBetterWeb.Web.MappingProfiles;

public class CoachingSessionProfile : Profile
{
  public CoachingSessionProfile()
  {
	  CreateMap<CoachingSession, CoachingSessionDto>();
  }
}
