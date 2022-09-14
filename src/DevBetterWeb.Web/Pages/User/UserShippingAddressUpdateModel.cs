using System.ComponentModel.DataAnnotations;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Web.Pages.User;

public class UserShippingAddressUpdateModel
{
	public UserShippingAddressUpdateModel()
	{
	}

	public UserShippingAddressUpdateModel(Member member)
	{
		this.Street = member.ShippingAddress.Street;
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
