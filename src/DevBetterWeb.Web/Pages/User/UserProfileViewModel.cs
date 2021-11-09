using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Web.Pages.User;

public class UserProfileViewModel
{

  [Required]
  public string? FirstName { get; set; }
  [Required]
  public string? LastName { get; set; }
  public string? Address { get; set; }
  [ValidUrl]
  public string? LinkedInUrl { get; set; }
  [ValidUrl]
  public string? TwitterUrl { get; set; }
  [ValidUrl]
  public string? GithubUrl { get; set; }
  [ValidUrl]
  public string? BlogUrl { get; set; }
  [ValidUrl]
  public string? TwitchUrl { get; set; }
  [ValidUrl]
  public string? YouTubeUrl { get; set; }
  [ValidUrl]
  public string? OtherUrl { get; set; }
  [ValidUrl]
  public string? CodinGameUrl { get; set; }
  public string? AboutInfo { get; set; }
  public string? PEFriendCode { get; set; }
  public string? PEUsername { get; set; }
  public string? DiscordUsername { get; set; }
  public List<Book> BooksRead { get; set; } = new List<Book>();
  public int? AddedBook { get; set; }
  public int? RemovedBook { get; set; }

  public UserProfileViewModel()
  {

  }

  public UserProfileViewModel(Member member)
  {
    FirstName = member.FirstName;
    LastName = member.LastName;
    BooksRead = member.BooksRead!;

    string valueToUseIfNull = "none";

    BlogUrl = member.BlogUrl ?? valueToUseIfNull;
    CodinGameUrl = member.CodinGameUrl ?? valueToUseIfNull;
    TwitchUrl = member.TwitchUrl ?? valueToUseIfNull;
    YouTubeUrl = member.YouTubeUrl ?? valueToUseIfNull;
    TwitterUrl = member.TwitterUrl ?? valueToUseIfNull;
    GithubUrl = member.GitHubUrl ?? valueToUseIfNull;
    LinkedInUrl = member.LinkedInUrl ?? valueToUseIfNull;
    OtherUrl = member.OtherUrl ?? valueToUseIfNull;
    AboutInfo = member.AboutInfo ?? valueToUseIfNull;
    Address = member.Address ?? valueToUseIfNull;
    PEFriendCode = member.PEFriendCode ?? valueToUseIfNull;
    PEUsername = member.PEUsername ?? valueToUseIfNull;
    DiscordUsername = member.DiscordUsername ?? valueToUseIfNull;

  }

}
