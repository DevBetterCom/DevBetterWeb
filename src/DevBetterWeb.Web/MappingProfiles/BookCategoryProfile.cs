using AutoMapper;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Web.Models;

namespace DevBetterWeb.Web.MappingProfiles;

public class BookCategoryProfile : Profile
{
  public BookCategoryProfile()
  {
	  CreateMap<BookCategory, BookCategoryDto>();
    CreateMap<BookCategoryDto, BookCategory>();
  }
}
