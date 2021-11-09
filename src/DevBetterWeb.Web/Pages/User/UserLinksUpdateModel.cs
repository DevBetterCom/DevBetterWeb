using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.DomainEvents;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Web.Pages.User;

public class UserLinksUpdateModel
{

  [ValidUrlContainingString("LinkedIn")]
  public string? LinkedInUrl { get; set; }
  [ValidUrlContainingString("Twitter")]
  public string? TwitterUrl { get; set; }
  [ValidUrlContainingString("GitHub")]
  public string? GithubUrl { get; set; }
  [ValidUrl]
  public string? BlogUrl { get; set; }
  [ValidUrlContainingString("Twitch")]
  public string? TwitchUrl { get; set; }
  [ValidUrlContainingString("YouTube")]
  public string? YouTubeUrl { get; set; }
  [ValidUrl]
  public string? OtherUrl { get; set; }
  [ValidUrlContainingString("CodinGame")]
  public string? CodinGameUrl { get; set; }

  public UserLinksUpdateModel()
  {

  }

  public UserLinksUpdateModel(Member member)
  {

    BlogUrl = member.BlogUrl;
    TwitchUrl = member.TwitchUrl;
    YouTubeUrl = member.YouTubeUrl;
    TwitterUrl = member.TwitterUrl;
    GithubUrl = member.GitHubUrl;
    LinkedInUrl = member.LinkedInUrl;
    OtherUrl = member.OtherUrl;
    CodinGameUrl = member.CodinGameUrl;
  }

}
