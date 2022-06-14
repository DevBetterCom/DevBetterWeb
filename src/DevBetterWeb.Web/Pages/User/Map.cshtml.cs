using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Web.Pages.User;

public class MapModel : PageModel
{
  private readonly ILogger<MapModel> _logger;

  public List<MapCoordinates> MemberCoordinates { get; set; } = new List<MapCoordinates>();
  public IRepository<Member> _memberRepository { get; }
  public IConfiguration _configuration { get; }

  public MapModel(IRepository<Member> memberRepository,
    IConfiguration configuration,
    ILogger<MapModel> logger)
  {
    _memberRepository = memberRepository;
    _configuration = configuration;
    _logger = logger;
  }

  public async Task OnGet(string userId)
  {
    var members = await _memberRepository.ListAsync();

    // Handle members that are in the database before map functionality is introduced, so they will not trigger the AddressUpdated event
    foreach (var member in members)
    {
      if (member.Address is not null && (member.CityLatitude is null || member.CityLongitude is null))
      {
        _logger.LogInformation($"Updating lat/long for {member.FirstName} {member.LastName}");
        // TODO: figure out if we need to update location data here
      }
    };

    MemberCoordinates = members.Where(m => m.CityLatitude is not null && m.CityLongitude is not null)
                                .Select(m => GetCoordinates(m, userId))
                                .ToList();
  }

  private MapCoordinates GetCoordinates(Member member, string userId)
  {
    Guard.Against.Null(member, nameof(member));
    var coordinates = new MapCoordinates((decimal)member.CityLatitude!, (decimal)member.CityLongitude!, member.UserFullName());
    if (member.CityLocation != null)
    {
      coordinates = new MapCoordinates(member.CityLocation.Latitude,
        member.CityLocation.Longitude, member.UserFullName());
    }
    if (member.UserId == userId) coordinates.IsClickedMember = true;
    return coordinates;
  }
}
