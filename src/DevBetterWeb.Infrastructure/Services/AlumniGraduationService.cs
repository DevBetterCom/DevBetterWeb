using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;

namespace DevBetterWeb.Infrastructure.Services
{
  public class AlumniGraduationService : IAlumniGraduationService
  {
    private const int DAYS_IN_TWO_YEARS = 365 * 2;

    private readonly IUserLookupService _userLookupService;
    private readonly IRepository<Member> _repository;
    private readonly IGraduationCommunicationsService _graduationCommunicationsService;
    private readonly IUserRoleManager _userRoleManager;

    public AlumniGraduationService(IUserLookupService userLookupService,
      IRepository<Member> repository,
      IGraduationCommunicationsService graduationCommunicationsService,
      IUserRoleManager userRoleManager)
    {
      _userLookupService = userLookupService;
      _repository = repository;
      _graduationCommunicationsService = graduationCommunicationsService;
      _userRoleManager = userRoleManager;
    }

    public async Task GraduateMembersIfNeeded(AppendOnlyStringList messages)
    {
      var members = await _repository.ListAsync();

      var membersToGraduate = await CheckIfAnyMemberGraduating(members);

      // add message indicating how many members should be graduated by the end of this flow
      if(membersToGraduate.Any())
      {
        string? membersToGraduateMessage;
        if (membersToGraduate.Count == 1)
        {
          membersToGraduateMessage = $"There is 1 member to graduate: {membersToGraduate[0].UserFullName()}.";
        }
        else
        {
          membersToGraduateMessage = $"There are {membersToGraduate.Count} members to graduate: ";
          foreach(var member in membersToGraduate)
          {
            membersToGraduateMessage += member.UserFullName() + ",";
          }
          membersToGraduateMessage = membersToGraduateMessage.Substring(0, membersToGraduateMessage.Length - 1);
          membersToGraduateMessage += ".";
        }
        messages.Append(membersToGraduateMessage);

        // graduate members
        var newMessages = await GraduateMembers(membersToGraduate);

        foreach(var message in newMessages)
        {
          messages.Append(message);
        }
      }
    }

    public async Task<List<Member>> CheckIfAnyMemberGraduating(List<Member> members)
    {
      var list = new List<Member>();

      foreach(var member in members)
      {
        if(member.TotalSubscribedDays() >= DAYS_IN_TWO_YEARS)
        {
          bool memberIsAlumnus = await _userLookupService.FindUserIsAlumniByUserIdAsync(member.UserId);

          if(!memberIsAlumnus)
          {
            list.Add(member);
          }
        }
      }
      return list;
    }

    public async Task<List<string>> GraduateMembers(List<Member> membersToGraduate)
    {
      var messages = new List<string>();
      foreach(var member in membersToGraduate)
      {
        await _userRoleManager.AddUserToRoleAsync(member.UserId, Constants.ALUMNI_ROLE_NAME);

        await _graduationCommunicationsService.SendGraduationCommunications(member);

        messages.Add($"{member.UserFullName()} has successfully graduated to Alumni status.");
      }
      return messages;
    }
  }
}
