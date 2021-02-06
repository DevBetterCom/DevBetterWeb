using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace DevBetterWeb.Infrastructure.Handlers
{
  public class MemberAddressUpdatedHandler : IHandle<MemberAddressUpdatedEvent>
  {
    private readonly ILogger<MemberAddressUpdatedHandler> _logger;

    public IMapCoordinateService _mapCoordinateService { get; }

    public MemberAddressUpdatedHandler(IMapCoordinateService mapCoordinateService,
      ILogger<MemberAddressUpdatedHandler> logger)
    {
      _mapCoordinateService = mapCoordinateService;
      _logger = logger;
    }

    public async Task Handle(MemberAddressUpdatedEvent addressUpdatedEvent)
    {
      var member = addressUpdatedEvent.Member;
      if (member.Address is null) return;

      _logger.LogInformation($"Parsing address for member {member.FirstName} {member.LastName}: {member.Address}");
      var addressParts = member.Address.Split(',');

      if (addressParts is not null)
      {
        
        //var cityString = addressParts[1];
        var cityDetailsAPIResponse = JObject.Parse(await _mapCoordinateService.GetMapCoordinates(member.Address));
        var latResponse = cityDetailsAPIResponse.SelectToken("results[0].geometry.location.lat");
        var lngResponse = cityDetailsAPIResponse.SelectToken("results[0].geometry.location.lng");

        member.CityLatitude = latResponse?.ToObject<decimal>();
        member.CityLongitude = lngResponse?.ToObject<decimal>();
      }
    }
  }
}


