using System;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Admin.Invitations;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
public class DeactivateModel : PageModel
{
	private readonly IRepository<Invitation> _invitationRepository;
	private readonly INewMemberService _newMemberService;

	public string Message { get; private set; } = "";

  public DeactivateModel(IRepository<Invitation> invitationRepository, INewMemberService newMemberService)
  {
	  _invitationRepository = invitationRepository;
	  _newMemberService = newMemberService;
  }

  public async Task OnGetAsync(string inviteCode)
  {
    var spec = new InvitationByInviteCodeSpec(inviteCode);
    var invitation = await _invitationRepository.FirstOrDefaultAsync(spec);

    Message = invitation != null ? 
	    $"If you are sure you want to deactivate {inviteCode} with email {invitation.Email} from DevBetter, click below." : 
	    "Invalid Link. Please try again.";
  }

  public async Task<PageResult> OnPost(string inviteCode)
  {
		var spec = new InvitationByInviteCodeSpec(inviteCode);
		var invitation = await _invitationRepository.FirstOrDefaultAsync(spec);

		try
    {
      await _newMemberService.DeactivateInviteAsync(invitation);
      Message = $"{inviteCode} [{invitation.Email}] has been deactivated from DevBetter.";
      return Page();
    }
    catch(Exception)
    {
      Message = "Attempt to deactivate invitation failed.";
      return Page();
    }
  }
}
