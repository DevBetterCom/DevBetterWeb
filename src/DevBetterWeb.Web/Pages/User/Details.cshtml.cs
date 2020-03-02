using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.User
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        public UserDetailsViewModel? UserDetailsViewModel { get; set; }
        private readonly IRepository _repository;

        public DetailsModel(IRepository repository)
        {
            _repository = repository;
        }

        public async Task OnGet(string userId)
        {
            // I don't think we need this - SAS
            //var user = await _userManager.FindByIdAsync(id);
            //if (user == null)
            //{
            //    BadRequest();
            //}

            // TODO: Add Specification support for this so we don't pull whole table into memory
            var member = (await _repository.ListAsync<Member>())
                .FirstOrDefault(member => member.UserId == userId);

            if (member == null)
            {
                // TODO: Add logging
                BadRequest();
            }

            UserDetailsViewModel = new UserDetailsViewModel(member!);
        }
    }
}