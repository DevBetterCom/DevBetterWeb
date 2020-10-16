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
#pragma warning disable CS0436 // Type conflicts with imported type
        public UserDetailsViewModel? UserDetailsViewModel { get; set; }
#pragma warning restore CS0436 // Type conflicts with imported type
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

            var spec = new MemberByUserIdWithBooksReadSpec(userId);
            var member = await _repository.GetAsync(spec);

            if (member == null)
            {
                // TODO: Add logging
                BadRequest();
            }

#pragma warning disable CS0436 // Type conflicts with imported type
            UserDetailsViewModel = new UserDetailsViewModel(member!);
#pragma warning restore CS0436 // Type conflicts with imported type
        }
    }
}
