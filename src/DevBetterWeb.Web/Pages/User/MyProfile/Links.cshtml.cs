using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.User
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
  public class MyProfileLinksModel : PageModel
    {
#nullable disable
        [BindProperty]
        public UserLinksUpdateModel UserLinksUpdateModel { get; set; }

#nullable enable

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemberRegistrationService _memberRegistrationService;
        private readonly IRepository _repository;
        private readonly AppDbContext _appDbContext;

        public MyProfileLinksModel(UserManager<ApplicationUser> userManager, 
            IMemberRegistrationService memberRegistrationService,
            IRepository repository, AppDbContext appDbContext)
        {
            _userManager = userManager;
            _memberRegistrationService = memberRegistrationService;
            _repository = repository;
            _appDbContext = appDbContext;
        }

        public async Task OnGetAsync()
        {
            var currentUserName = User.Identity!.Name;
            var applicationUser = await _userManager.FindByNameAsync(currentUserName);

            var spec = new MemberByUserIdWithBooksReadSpec(applicationUser.Id);
            var member = await _repository.GetAsync(spec);

            //var books = await _appDbContext.Books
            //    .Include(book => book.MembersWhoHaveRead)
            //    .ToListAsync();

            //var members = await _appDbContext.Members
            //    .Include(member => member.BooksRead)
            //    .ToListAsync();



            if (member == null)
            {
                member = await _memberRegistrationService.RegisterMemberAsync(applicationUser.Id);
            }

            UserLinksUpdateModel = new UserLinksUpdateModel(member);
        }

        public async Task OnPost()
        {
            if (!ModelState.IsValid) return;
            // TODO: consider only getting the user alias not the whole URL for social media links
            // TODO: assess risk of XSS attacks and how to mitigate

            var currentUserName = User.Identity!.Name;
            var applicationUser = await _userManager.FindByNameAsync(currentUserName);

            var spec = new MemberByUserIdWithBooksReadSpec(applicationUser.Id);
            var member = await _repository.GetAsync(spec);

            member.UpdateLinks(UserLinksUpdateModel.BlogUrl, UserLinksUpdateModel.CodinGameUrl, UserLinksUpdateModel.GithubUrl, UserLinksUpdateModel.LinkedInUrl,
                UserLinksUpdateModel.OtherUrl, UserLinksUpdateModel.TwitchUrl, UserLinksUpdateModel.YouTubeUrl, UserLinksUpdateModel.TwitterUrl);

            await _repository.UpdateAsync(member);
        }
    }
}
