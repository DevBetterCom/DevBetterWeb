using CleanArchitecture.Core.Entities;
using CleanArchitecture.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace CleanArchitecture.Web.Pages.ArchivedVideos
{
    public class IndexModel : PageModel
    {
        private readonly IRepository _repository;

        public List<ArchiveVideo> Videos { get; set; }

        public IndexModel(IRepository repository)
        {
            _repository = repository;
        }

        public void OnGet()
        {
            Videos = _repository.List<ArchiveVideo>();
        }
    }
}
