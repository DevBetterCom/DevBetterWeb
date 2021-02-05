using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using Newtonsoft.Json.Linq;

namespace DevBetterWeb.Infrastructure.Handlers
{
  public class MemberAddressUpdatedHandler : IHandle<MemberAddressUpdatedEvent>
  {
    public IMapCoordinateService _mapCoordinateService { get; }

    public MemberAddressUpdatedHandler(IMapCoordinateService mapCoordinateService)
    {
      _mapCoordinateService = mapCoordinateService;
    }

    public async Task Handle(MemberAddressUpdatedEvent addressUpdatedEvent)
    {
      var member = addressUpdatedEvent.Member;
      var addressParts = member.Address?.Split(',');

      if (addressParts is not null)
      {
        var cityString = addressParts[1];
        var cityDetailsAPIResponse = JObject.Parse(await _mapCoordinateService.GetMapCoordinates(cityString));
        var latResponse = cityDetailsAPIResponse.SelectToken("results[0].geometry.location.lat");
        var lngResponse = cityDetailsAPIResponse.SelectToken("results[0].geometry.location.lng");

        member.CityLatitude = latResponse?.ToObject<decimal>();
        member.CityLongitude = lngResponse?.ToObject<decimal>();
      }
    }
  }
}


