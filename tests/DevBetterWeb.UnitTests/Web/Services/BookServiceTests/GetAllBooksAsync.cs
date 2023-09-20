using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Web.Services;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace DevBetterWeb.UnitTests.Web.Services.FilteredBookDetailsTests;

public class GetAllBooksAsync
{
	private readonly IRepository<Book> _bookRepositoryMock;
	private readonly BookService _bookService;

	public GetAllBooksAsync()
	{
		_bookRepositoryMock = Substitute.For<IRepository<Book>>();
		_bookService = new BookService(_bookRepositoryMock);
	}

	[Fact]
	public async Task GetsAllBooks()
	{
		// Arrange
		var expectedBooks = new List<Book>
		{
			new Book { Title = "Book1", Author = "Author1" }, new Book { Title = "Book2", Author = "Author2" }
		};

		_bookRepositoryMock.ListAsync(Arg.Any<BooksOrderedByReadsSpec>(), Arg.Any<CancellationToken>())
			.Returns(expectedBooks);

		// Act
		var actualBooksViewModel = await _bookService.GetAllBooksAsync();

		// Assert
		await _bookRepositoryMock.Received(1).ListAsync(Arg.Any<BooksOrderedByReadsSpec>(), Arg.Any<CancellationToken>());

		Assert.Equal(expectedBooks.Count, actualBooksViewModel.Count);
		foreach (var expectedBook in expectedBooks)
		{
			Assert.Contains(actualBooksViewModel,
				bvm => bvm.Title == expectedBook.Title && bvm.Author == expectedBook.Author);
		}
	}

	[Fact]
	public async Task ReturnsEmptyListGivenNoBooks()
	{
		// Arrange
		_bookRepositoryMock.ListAsync(Arg.Any<BooksOrderedByReadsSpec>(), Arg.Any<CancellationToken>())
			.Returns(new List<Book>());

		// Act
		var actualBooksViewModel = await _bookService.GetAllBooksAsync();

		// Assert
		await _bookRepositoryMock.Received(1).ListAsync(Arg.Any<BooksOrderedByReadsSpec>(), Arg.Any<CancellationToken>());

		Assert.Empty(actualBooksViewModel);
	}

	[Fact]
	public async Task ReturnsBooksInCorrectOrder()
	{
		// Arrange
		var book1 = new Book
		{
			Title = "Book1", Author = "Author1", MembersWhoHaveRead = new List<Member> { new Member(), }, BookCategoryId = 1
		};
		var book2 = new Book
		{
			Title = "Book2",
			Author = "Author2",
			MembersWhoHaveRead = new List<Member> { new Member(), new Member(), },
			BookCategoryId = 1
		};
		var book3 = new Book
		{
			Title = "Book3", Author = "Author3", MembersWhoHaveRead = new List<Member> { new Member(), }, BookCategoryId = 2
		};

		var expectedBooks = new List<Book> { book1, book2, book3 };
		_bookRepositoryMock.ListAsync(Arg.Any<BooksOrderedByReadsSpec>(), Arg.Any<CancellationToken>())
			.Returns(expectedBooks);

		// Act
		var actualBooksViewModel = await _bookService.GetAllBooksAsync();

		// Assert
		await _bookRepositoryMock.Received(1).ListAsync(Arg.Any<BooksOrderedByReadsSpec>(), Arg.Any<CancellationToken>());

		Assert.Equal(expectedBooks.Count, actualBooksViewModel.Count);
		Assert.Equal(book2.Title, actualBooksViewModel[0].Title); // Book2 has higher ReadsCount
		Assert.Equal(book1.Title, actualBooksViewModel[1].Title); // Book1 has lower ReadsCount
		Assert.Equal(book3.Title, actualBooksViewModel[2].Title); // Book3 has higher BookCategoryId
	}

	[Fact]
	public async Task ThrowsExceptionWhenRepositoryFails()
	{
		// Arrange
		_bookRepositoryMock.ListAsync(Arg.Any<BooksOrderedByReadsSpec>(), Arg.Any<CancellationToken>())
			.Throws(new System.Exception());

		// Act & Assert
		await Assert.ThrowsAsync<System.Exception>(() => _bookService.GetAllBooksAsync());
	}
}
