using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Web.Pages.User;

public class UserPersonalUpdateModel : IValidatableObject
{

	[Required]
	public string? FirstName { get; set; }
	[Required]
	public string? LastName { get; set; }
	public string? Address { get; set; }
	public string? City { get; set; }
	public string? State { get; set; }
	public string? Country { get; set; }
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
		if (member.ShippingAddress != null)
		{
			Address = member.ShippingAddress.Street;
			City = member.ShippingAddress.City;
			Country = member.ShippingAddress.Country;
			State = member.ShippingAddress.State;
			PostalCode = member.ShippingAddress.PostalCode;
		}
		BirthdayDay = member.Birthday?.Day;
		BirthdayMonth = member.Birthday?.Month;
		Email = member.Email;
		PEFriendCode = member.PEFriendCode;
		PEUsername = member.PEUsername;
		DiscordUsername = member.DiscordUsername;
	}

	public bool HasAnyAddressField() =>
		!string.IsNullOrWhiteSpace(Address) ||
		!string.IsNullOrWhiteSpace(City) ||
		!string.IsNullOrWhiteSpace(Country) ||
		!string.IsNullOrWhiteSpace(PostalCode) ||
		!string.IsNullOrWhiteSpace(State);

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (!HasAnyAddressField()) yield break;

		if (string.IsNullOrWhiteSpace(Address))
			yield return new ValidationResult("The Address field is required when providing address information.", new[] { nameof(Address) });

		if (string.IsNullOrWhiteSpace(City))
			yield return new ValidationResult("The City field is required when providing address information.", new[] { nameof(City) });

		if (string.IsNullOrWhiteSpace(Country))
			yield return new ValidationResult("The Country field is required when providing address information.", new[] { nameof(Country) });

		if (string.IsNullOrWhiteSpace(PostalCode))
			yield return new ValidationResult("The Postal Code field is required when providing address information.", new[] { nameof(PostalCode) });
	}
}
