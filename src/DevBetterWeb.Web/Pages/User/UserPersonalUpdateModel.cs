using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Web.Pages.User;

public class UserPersonalUpdateModel
{

	[Required]
	public string? FirstName { get; set; }
	[Required]
	public string? LastName { get; set; }
	[Required]
	public string? Address { get; set; }
	[Required]
	public string? City { get; set; }
	[Required]
	public string? Country { get; set; }
	[Required]
	public string? PostalCode { get; set; }
	[Range(1, 31)]
  [BirthdayDay]
	public int? BirthdayDay { get; set; }
	[Range(1, 12)]
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
		City = member.City;
		Country = member.Country;
		PostalCode = member.PostalCode;
		BirthdayDay = member.Birthday?.Day;
		BirthdayMonth = member.Birthday?.Month;
		Email = member.Email;
		PEFriendCode = member.PEFriendCode;
		PEUsername = member.PEUsername;
		DiscordUsername = member.DiscordUsername;
	}
}
