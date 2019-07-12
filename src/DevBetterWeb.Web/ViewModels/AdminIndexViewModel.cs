using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.ViewModels
{
    public class AdminIndexViewModel
    {
        public List<ApplicationUser> Users { get; set; }
        public List<IdentityRole> Roles  { get; set; }

    }
}
