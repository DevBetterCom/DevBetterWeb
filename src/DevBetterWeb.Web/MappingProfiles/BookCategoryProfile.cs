using System.Linq;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Web.Models;

namespace DevBetterWeb.Web.MappingProfiles;

public class BookCategoryProfile : Profile
{
  public BookCategoryProfile()
  {
    CreateMap<BookCategory, BookCategoryDto>()
	    .ForPath(dest => dest.Members,
		    opt => opt.MapFrom(source => source.Books!.Where(b => b.BookCategoryId == source.Id).SelectMany(book => book.MembersWhoHaveRead!.Where(m => m.RoleName == AuthConstants.Roles.MEMBERS)).Distinct().ToList()))
	    .ForPath(dest => dest.Alumnus,
	    opt => opt.MapFrom(source => source.Books!.Where(b => b.BookCategoryId == source.Id).SelectMany(book => book.MembersWhoHaveRead!.Where(m => m.RoleName == AuthConstants.Roles.ALUMNI)).Distinct().ToList()));
		CreateMap<BookCategoryDto, BookCategory>();
  }
}
