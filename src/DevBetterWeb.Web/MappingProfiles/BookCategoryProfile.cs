using System.Linq;
using AutoMapper;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Web.Models;

namespace DevBetterWeb.Web.MappingProfiles;

public class BookCategoryProfile : Profile
{
  public BookCategoryProfile()
  {
    CreateMap<BookCategory, BookCategoryDto>()
	    .ForPath(dest => dest.Members,
		    opt => opt.MapFrom(source => source.Books!.SelectMany(book => book.MembersWhoHaveRead!).Distinct().ToList()));
		CreateMap<BookCategoryDto, BookCategory>();
  }
}
