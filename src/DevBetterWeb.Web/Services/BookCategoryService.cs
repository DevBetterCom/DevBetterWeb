using AutoMapper;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Services;

public class BookCategoryService : IBookCategoryService
{
	private readonly IMapper _mapper;
	private readonly IRepository<BookCategory> _bookCategoryRepository;

	public BookCategoryService(IMapper mapper, IRepository<BookCategory> bookCategoryRepository)
	{
		_mapper = mapper;
		_bookCategoryRepository = bookCategoryRepository;
	}

	public async Task<List<BookCategoryDto>> GetBookCategoriesAsync()
	{
		var spec = new BookCategoriesSpec();
		var bookCategoriesEntity = await _bookCategoryRepository.ListAsync(spec);
		return _mapper.Map<List<BookCategoryDto>>(bookCategoriesEntity);
	}
}
