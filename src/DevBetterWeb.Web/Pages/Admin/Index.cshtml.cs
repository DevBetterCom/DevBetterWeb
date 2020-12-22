using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Admin
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
  public class IndexModel : PageModel
  {
  }
}
