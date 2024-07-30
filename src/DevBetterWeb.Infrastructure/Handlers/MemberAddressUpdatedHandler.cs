﻿using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace DevBetterWeb.Infrastructure.Handlers;

public class MemberAddressUpdatedHandler : IHandle<MemberAddressUpdatedEvent>
{
  private readonly ILogger<MemberAddressUpdatedHandler> _logger;

  private readonly AdminUpdatesWebhook _webhook;
  //private readonly IRepository _repository;

  public IMapCoordinateService _mapCoordinateService { get; }

  public MemberAddressUpdatedHandler(IMapCoordinateService mapCoordinateService,
    ILogger<MemberAddressUpdatedHandler> logger,
    AdminUpdatesWebhook webhook
		//IRepository repository
		)
  {
    _mapCoordinateService = mapCoordinateService;
    _logger = logger;
    _webhook = webhook;
    //_repository = repository;
  }

  public async Task Handle(MemberAddressUpdatedEvent addressUpdatedEvent)
  {
    var member = addressUpdatedEvent.Member;
    if (member.ShippingAddress is null) return;

    _logger.LogInformation($"Parsing address for member {member.FirstName} {member.LastName}: {member.ShippingAddress}");
    var addressParts = member.ShippingAddress.ToString().Split(',');

    if (addressParts is not null)
    {
      string responseString = await _mapCoordinateService.GetMapCoordinates(member.ShippingAddress.ToString());
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

        var message = $"Member {member.UserFullName()} change the address to {member.ShippingAddress}";
        await _webhook.SendAsync(message);

				//await _repository.UpdateAsync(member);
			}
    }
  }
}


