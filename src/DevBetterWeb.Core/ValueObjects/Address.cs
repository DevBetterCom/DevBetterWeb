using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace DevBetterWeb.Core.ValueObjects
{
  public class Address : ValueObject
  {
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string ZipCode { get; }
    public string Country { get; }

    public Address(string street, 
      string city,
      string state,
      string zipCode,
      string country)
    {
      Street = street;
      City = city;
      State = state;
      ZipCode = zipCode;
      Country = country;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return Street;
      yield return City;
      yield return State;
      yield return ZipCode;
      yield return Country;
    }
  }
}
