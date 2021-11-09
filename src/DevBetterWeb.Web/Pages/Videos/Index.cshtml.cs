using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Web.Models.Vimeo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Videos;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
public class IndexModel : PageModel
{
}
