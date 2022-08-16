using AutoMapper;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Web.Models;

namespace DevBetterWeb.Web.MappingProfiles;

public class BookProfile : Profile
{
  public BookProfile()
  {
    CreateMap<Book, BookDto>()
	    .ForPath(dest => dest.CategoryTitle,
		    opt => opt.MapFrom(source => source.BookCategory!.Title))
	    .ForPath(dest => dest.MembersWhoHaveReadCount,
		    opt => opt.MapFrom(source => source.MembersWhoHaveRead!.Count))
	    .ForPath(dest => dest.TitleWithAuthor,
		    opt => opt.MapFrom(source => source.ToString()))
			.ForPath(dest => dest.MemberWhoUploaded,
				opt => opt.MapFrom(source => source.MemberWhoUpload!.ToString()));
		CreateMap<BookDto, Book>();
  }
}
