using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.ViewModels
{
    public class RoleViewModel
    {
        public IdentityRole Role { get; set; }
        public List<ApplicationUser> UsersInRole { get; set; }
        public List<SelectListItem> UsersNotInRole { get; set; }
    }
}
