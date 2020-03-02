using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using Microsoft.AspNetCore.Authorization;
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

            var spec = new MemberByUserIdSpec(userId);
            var member = await _repository.GetBySpecAsync(spec);

            if (member == null)
            {
                // TODO: Add logging
                BadRequest();
            }

            UserDetailsViewModel = new UserDetailsViewModel(member!);
        }
    }
}