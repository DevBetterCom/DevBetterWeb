using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using CSharpFunctionalExtensions;

namespace DevBetterWeb.Core.ValueObjects;

public class Address : ValueObject
{
  public string Street { get; private set; }
  public string City { get; private set; }
  public string State { get; private set; }
  public string PostalCode { get; private set; }
  public string Country { get; private set; }

  public Address(string street,
    string city,
    string state,
    string postalCode,
    string country)
  {
    Street = street;
    City = city;
    State = state;
    PostalCode = postalCode;
    Country = country;
  }

  protected override IEnumerable<IComparable> GetEqualityComponents()
  {
    yield return Street;
    yield return City;
    yield return State;
    yield return PostalCode;
    yield return Country;
  }

  public string ToCityStateCountryString()
  {
    string country = (Country ?? "".Trim());
    if (country.Length == 0) return "";

    string state = (State ?? "").Trim();
    string city = (City ?? "").Trim();
    var tokens = new List<string>();
    if (city.Length > 0) tokens.Add(city);
    if (state.Length > 0) tokens.Add(state);
    tokens.Add(country);

    return string.Join(',', tokens);
	}

  public bool Update(string street, string city, string state, string postalCode, string country)
  {
	  var isUpdated = false;

	  if (Street != street)
	  {
			isUpdated = true;
			Street = street;
	  }

	  if (City != city)
	  {
		  isUpdated = true;
		  City = city;
	  }

	  if (State != state)
	  {
		  isUpdated = true;
		  State = state;
	  }

	  if (Country != country)
	  {
		  isUpdated = true;
		  Country = country;
	  }

	  if (PostalCode != postalCode)
	  {
		  isUpdated = true;
		  PostalCode = postalCode;
	  }


		return isUpdated;
  }
}
