using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace DevBetterWeb.Infrastructure.Handlers;

public class MemberAddressUpdatedHandler : IHandle<MemberAddressUpdatedEvent>
{
  private readonly ILogger<MemberAddressUpdatedHandler> _logger;
  //private readonly IRepository _repository;

  public IMapCoordinateService _mapCoordinateService { get; }

  public MemberAddressUpdatedHandler(IMapCoordinateService mapCoordinateService,
    ILogger<MemberAddressUpdatedHandler> logger
    //IRepository repository
    )
  {
    _mapCoordinateService = mapCoordinateService;
    _logger = logger;
    //_repository = repository;
  }

  public async Task Handle(MemberAddressUpdatedEvent addressUpdatedEvent)
  {
    var member = addressUpdatedEvent.Member;
    if (member.Address is null) return;

    _logger.LogInformation($"Parsing address for member {member.FirstName} {member.LastName}: {member.Address}");
    var addressParts = member.Address.Split(',');

    if (addressParts is not null)
    {
      string responseString = await _mapCoordinateService.GetMapCoordinates(member.Address);
      if (!string.IsNullOrEmpty(responseString))
      {
        _logger.LogInformation($"API Response: {responseString}");
        var cityDetailsAPIResponse = JObject.Parse(responseString);
        var latResponse = cityDetailsAPIResponse.SelectToken("results[0].geometry.location.lat");
        var lngResponse = cityDetailsAPIResponse.SelectToken("results[0].geometry.location.lng");

        decimal? latitude = latResponse?.ToObject<decimal>();
        decimal? longitude = lngResponse?.ToObject<decimal>();
        if (latitude.HasValue && longitude.HasValue)
        {
          member.CityLatitude = latitude;
          member.CityLongitude = longitude;
          _logger.LogInformation($"Set lat/long to {latitude}/{longitude}.");
        }
        //await _repository.UpdateAsync(member);
      }
    }
  }
}


