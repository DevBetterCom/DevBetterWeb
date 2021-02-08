using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace DevBetterWeb.Core.ValueObjects
{
  public class Geolocation : ValueObject
  {
    public decimal Latitude { get; }
    public decimal Longitude { get; }

    public Geolocation(decimal latitude, decimal longitude)
    {
      Latitude = latitude;
      Longitude = longitude;
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return Latitude;
      yield return Longitude;
    }
  }
}
