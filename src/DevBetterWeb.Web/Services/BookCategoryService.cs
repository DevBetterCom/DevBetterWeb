using AutoMapper;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Services;

/// <summary>
/// Service for handling operations related to book categories.
/// </summary>
public class BookCategoryService : IBookCategoryService
{
	private readonly IMapper _mapper;
	private readonly IRepository<BookCategory> _bookCategoryRepository;

	/// <summary>
	/// Initializes a new instance of the <see cref="BookCategoryService"/> class.
	/// </summary>
	/// <param name="mapper">The object mapper.</param>
	/// <param name="bookCategoryRepository">The book category repository.</param>
	public BookCategoryService(IMapper mapper, IRepository<BookCategory> bookCategoryRepository)
	{
		_mapper = mapper;
		_bookCategoryRepository = bookCategoryRepository;
	}


	/// <summary>
	/// Asynchronously retrieves all book categories.
	/// </summary>
	/// <returns>A list of book categories data transfer objects.</returns>
	public async Task<List<BookCategoryDto>> GetBookCategoriesAsync()
	{
		var spec = new BookCategoriesSpec();
		var bookCategoriesEntity = await _bookCategoryRepository.ListAsync(spec);
		return _mapper.Map<List<BookCategoryDto>>(bookCategoriesEntity);
	}
}
