using System;
using System.Linq;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using Xunit;

namespace DevBetterWeb.UnitTests.Core.Entities.MemberTests;

public class MemberUpdateAddress
{
  private string _initialStreet = "";
  private string _initialCity = "";
  private string _initialCountry = "";
  private string _initialPostalCode = "";
  private string _initialState = "";

  private Member GetMemberWithDefaultAddress()
  {
    _initialStreet = Guid.NewGuid().ToString();
    _initialCity = Guid.NewGuid().ToString();
    _initialCountry = Guid.NewGuid().ToString();
    _initialPostalCode = Guid.NewGuid().ToString();
    _initialState = Guid.NewGuid().ToString();

    var member = MemberHelpers.CreateWithDefaultConstructor();
    member.UpdateShippingAddress(_initialStreet, _initialCity, _initialState, _initialPostalCode, _initialCountry);
    member.Events.Clear();

    return member;
  }

  [Fact]
  public void SetsAddress()
  {
    string newStreet = Guid.NewGuid().ToString();
    string newCity = Guid.NewGuid().ToString();
    string newCountry = Guid.NewGuid().ToString();
    string newPostalCode = Guid.NewGuid().ToString();
    string newState = Guid.NewGuid().ToString();

		var member = GetMemberWithDefaultAddress();
		member.UpdateShippingAddress(newStreet, newCity, newState, newPostalCode, newCountry);

		Assert.Equal(newStreet, member.ShippingAddress?.Street);
		Assert.Equal(newCity, member.ShippingAddress?.City);
		Assert.Equal(newCountry, member.ShippingAddress?.Country);
		Assert.Equal(newPostalCode, member.ShippingAddress?.PostalCode);
		Assert.Equal(newState, member.ShippingAddress?.State);
  }

  [Fact]
  public void RecordsEventIfAddressChanges()
  {
    string newStreet = Guid.NewGuid().ToString();
    string newCity = Guid.NewGuid().ToString();
    string newCountry = Guid.NewGuid().ToString();
    string newPostalCode = Guid.NewGuid().ToString();
    string newState = Guid.NewGuid().ToString();

		var member = GetMemberWithDefaultAddress();
		member.UpdateShippingAddress(newStreet, newCity, newState, newPostalCode, newCountry);
		var eventCreated = (MemberAddressUpdatedEvent)member.Events.First();

    Assert.Same(member, eventCreated.Member);
		Assert.Equal(newStreet, member.ShippingAddress?.Street);
		Assert.Equal(newCity, member.ShippingAddress?.City);
		Assert.Equal(newCountry, member.ShippingAddress?.Country);
		Assert.Equal(newPostalCode, member.ShippingAddress?.PostalCode);
		Assert.Equal(newState, member.ShippingAddress?.State);
	}

  [Fact]
  public void RecordsNoEventIfAddressDoesNotChange()
  {
    var member = GetMemberWithDefaultAddress();
		member.UpdateShippingAddress(_initialStreet, _initialCity, _initialState, _initialPostalCode, _initialCountry);

		Assert.Empty(member.Events);
  }

  [Fact]
  public void RecordsEventWithAppendedDetailsIfOtherThingsChanged()
  {
    string newDiscord = Guid.NewGuid().ToString();

    var member = GetMemberWithDefaultAddress();
    member.UpdateName("kylo", "ren");
    member.UpdateDiscord(newDiscord);
    var eventCreated = (MemberUpdatedEvent)member.Events.First();

    Assert.Same(member, eventCreated.Member);
    Assert.Equal("Name,Discord Username", eventCreated.UpdateDetails);
  }
}
