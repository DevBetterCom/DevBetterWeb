using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Web.Pages.User
{
    [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS)]
    public class MyProfileModel : PageModel
    {
#nullable disable
        [BindProperty]
        public UserProfileUpdateModel UserProfileUpdateModel { get; set; }
        public List<Book> Books = new List<Book> { new Book { Author = "Test", Title = "TestTitle" } };
#nullable enable

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemberRegistrationService _memberRegistrationService;
        private readonly IRepository _repository;
        private readonly AppDbContext _appDbContext;

        public MyProfileModel(UserManager<ApplicationUser> userManager, 
            IMemberRegistrationService memberRegistrationService,
            IRepository repository, AppDbContext appDbContext)
        {
            _userManager = userManager;
            _memberRegistrationService = memberRegistrationService;
            _repository = repository;
            _appDbContext = appDbContext;
            Books = _appDbContext.Books.ToList();

        }

        public async Task OnGetAsync()
        {
            var currentUserName = User.Identity.Name;
            var applicationUser = await _userManager.FindByNameAsync(currentUserName);

            var spec = new MemberByUserIdSpec(applicationUser.Id);
            var member = await _repository.GetAsync(spec);

            if (member == null)
            {
                member = await _memberRegistrationService.RegisterMemberAsync(applicationUser.Id);
            }

            UserProfileUpdateModel = new UserProfileUpdateModel(member);
        }

        public async Task OnPost()
        {
            if (!ModelState.IsValid) return;
            // TODO: consider only getting the user alias not the whole URL for social media links
            // TODO: assess risk of XSS attacks and how to mitigate

            var currentUserName = User.Identity.Name;
            var applicationUser = await _userManager.FindByNameAsync(currentUserName);

            var spec = new MemberByUserIdSpec(applicationUser.Id);
            var member = await _repository.GetAsync(spec);

            member.UpdateName(UserProfileUpdateModel.FirstName, UserProfileUpdateModel.LastName); 
            member.UpdatePEInfo(UserProfileUpdateModel.PEFriendCode, UserProfileUpdateModel.PEUsername);
            member.UpdateAboutInfo(UserProfileUpdateModel.AboutInfo);
            member.UpdateAddress(UserProfileUpdateModel.Address);
            member.UpdateLinks(UserProfileUpdateModel.BlogUrl, UserProfileUpdateModel.GithubUrl, UserProfileUpdateModel.LinkedInUrl,
                UserProfileUpdateModel.OtherUrl, UserProfileUpdateModel.TwitchUrl, UserProfileUpdateModel.YouTubeUrl, UserProfileUpdateModel.TwitterUrl);
            member.UpdateBooks(UserProfileUpdateModel.BooksRead);

            await _repository.UpdateAsync(member);
        }
    }
}