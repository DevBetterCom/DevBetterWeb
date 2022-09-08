using System.ComponentModel.DataAnnotations;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Web.Pages.User;

public class UserPersonalUpdateModel
{

  [Required]
  public string? FirstName { get; set; }
  [Required]
  public string? LastName { get; set; }
  public string? Address { get; set; }
  public int? BirthdayDay { get; set; }
  public int? BirthdayMonth { get; set; }
  public string? UserId { get; set; }
  public string? Email { get; set; }
  public string? AboutInfo { get; set; }
  public string? PEFriendCode { get; set; }
  public string? PEUsername { get; set; }
  [ValidDiscordUsername]
  public string? DiscordUsername { get; set; }

  public UserPersonalUpdateModel()
  {

  }

  public UserPersonalUpdateModel(Member member)
  {
	  UserId = member.UserId;
	  AboutInfo = member.AboutInfo;
    FirstName = member.FirstName;
    LastName = member.LastName;
    Address = member.Address;
    BirthdayDay = member.Birthday?.Day;
    BirthdayMonth = member.Birthday?.Month;
    Email = member.Email;
    PEFriendCode = member.PEFriendCode;
    PEUsername = member.PEUsername;
    DiscordUsername = member.DiscordUsername;
  }
}
