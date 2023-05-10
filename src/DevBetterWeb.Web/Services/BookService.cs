using AutoMapper;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Pages.Leaderboard;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Services;

/// <summary>
/// Service for handling operations related to book categories.
/// </summary>
public class BookService : IBookService
{
	private readonly IRepository<Book> _bookRepository;

	/// <summary>
	/// Initializes a new instance of the <see cref="BookService"/> class.
	/// </summary>
	/// <param name="bookRepository">The book category repository.</param>
	public BookService(IRepository<Book> bookRepository)
	{
		_bookRepository = bookRepository;
	}


	public async Task<BookDetailsViewModel?> GetBookByIdAsync(int bookId)
	{
		var spec = new BookByIdWithMembersSpec(bookId);
		var book = await _bookRepository.FirstOrDefaultAsync(spec);
		if (book == null) return null;

		var bookDetailsViewModel = new BookDetailsViewModel(book);

		return bookDetailsViewModel;
	}
}
