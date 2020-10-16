using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Web.Pages.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Leaderboard
{
    [Authorize]
    public class BookDetailsModel : PageModel
    {
        public BookDetailsViewModel? BookDetailsViewModel { get; set; }
        private readonly IRepository _repository;

        public BookDetailsModel(IRepository repository)
        {
            _repository = repository;
        }

        public async Task OnGet(string bookId)
        {
            // I don't think we need this - SAS
            //var user = await _userManager.FindByIdAsync(id);
            //if (user == null)
            //{
            //    BadRequest();
            //}

            var spec = new BookByIdWithMembersSpec(int.Parse(bookId));
            var book = await _repository.GetAsync(spec);

            if (book == null)
            {
                // TODO: Add logging
                BadRequest();
            }

            BookDetailsViewModel = new BookDetailsViewModel(book!);
        }
    }
}
