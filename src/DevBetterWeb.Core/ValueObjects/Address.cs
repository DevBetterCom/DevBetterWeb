using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace DevBetterWeb.Core.ValueObjects;

public class Address : ValueObject
{
  public string Street { get; }
  public string City { get; }
  public string State { get; }
  public string PostalCode { get; }
  public string Country { get; }

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

  protected override IEnumerable<object> GetEqualityComponents()
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
}
