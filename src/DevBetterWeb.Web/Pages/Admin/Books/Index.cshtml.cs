using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using DevBetterWeb.Core;

namespace DevBetterWeb.Web.Pages.Admin.Books
{
    [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]

    public class IndexModel : PageModel
    {
        private readonly DevBetterWeb.Infrastructure.Data.AppDbContext _context;

        public IndexModel(DevBetterWeb.Infrastructure.Data.AppDbContext context)
        {
            _context = context;
        }

        public IList<Book>? Book { get;set; }

        public async Task OnGetAsync()
        {
            Book = await _context.Books!.AsQueryable().ToListAsync();
        }
    }
}
