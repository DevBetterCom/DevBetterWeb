using System.ComponentModel.DataAnnotations;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Web.Pages.User;

public class UserShippingAddressUpdateModel
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public UserShippingAddressUpdateModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	{
	}

	public UserShippingAddressUpdateModel(Member member)
	{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
		this.Street = member.ShippingAddress.Street;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
		this.City = member.ShippingAddress.City;
		this.State = member.ShippingAddress.State;
		this.PostalCode = member.ShippingAddress.PostalCode;
		this.Country = member.ShippingAddress.Country;
	}

	[Required]
	public string City { get; set; }

	[Required]
	public string Country { get; set; }

	public string State { get; set; }
	public string Street { get; set; }

	public string PostalCode { get; set; }

}
