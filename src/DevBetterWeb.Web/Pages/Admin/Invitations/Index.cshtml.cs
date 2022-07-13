using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Admin.Invitations;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
public class IndexModel : PageModel
{
	public IList<InvitationDto> Invitations { get; set; } = new List<InvitationDto>();
	
  private readonly IRepository<Invitation> _invitationRepository;
  private readonly IMapper _mapper;

  public IndexModel(IRepository<Invitation> invitationRepository, IMapper mapper)
  {
    _invitationRepository = invitationRepository;
    _mapper = mapper;
  }

  public async Task<IActionResult> OnGetAsync()
  {
	  var invitations = await _invitationRepository.ListAsync();
	  Invitations = _mapper.Map<List<InvitationDto>>(invitations);

	  return Page();
  }
}
