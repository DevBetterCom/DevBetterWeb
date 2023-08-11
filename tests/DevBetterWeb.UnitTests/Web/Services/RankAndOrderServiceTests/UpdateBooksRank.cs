using System;
using System.Collections.Generic;
using DevBetterWeb.Core;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Services;
using Xunit;
using Moq;
using System.Linq;

namespace DevBetterWeb.UnitTests.Web.Services.RankAndOrderServiceTests;

public class UpdateBooksRank
{
	private readonly Mock<IRankingService> _mockRankingService;
	private readonly RankAndOrderService _rankAndOrderService;

	public UpdateBooksRank()
	{
		_mockRankingService = new Mock<IRankingService>();
		var mockMemberService = new Mock<IMemberService>();
		_rankAndOrderService = new RankAndOrderService(_mockRankingService.Object, mockMemberService.Object);
	}

	private List<BookCategoryDto> GetTestBookCategories()
	{
		return new List<BookCategoryDto>
				{
						new BookCategoryDto
						{
								Id = 1,
								Books = new List<BookDto>
								{
										new BookDto
										{
												Id = 1,
												MembersWhoHaveRead = new List<MemberForBookDto>
												{
														new MemberForBookDto { Id = 1, RoleName = AuthConstants.Roles.ALUMNI, UserId = "1", FullName = "Alumni1" },
														new MemberForBookDto { Id = 2, RoleName = AuthConstants.Roles.MEMBERS, UserId = "2", FullName = "Member1" },
														new MemberForBookDto { Id = 3, RoleName = AuthConstants.Roles.ALUMNI, UserId = "3", FullName = "Alumni2" }
												}
										}
								}
						}
				};
	}

	[Fact]
	public void CallCalculateBookRankForEachCategoryGivenCategoriesWithBooks()
	{
		// Arrange
		var bookCategories = GetTestBookCategories();

		// Act
		_rankAndOrderService.UpdateBooksRank(bookCategories);

		// Assert
		_mockRankingService.Verify(rs => rs.CalculateBookRank(It.IsAny<List<BookDto>>()), Times.Exactly(bookCategories.Count));
	}

	[Fact]
	public void NotCallCalculateBookRankGivenCategoryWithoutBooks()
	{
		// Arrange
		var bookCategories = new List<BookCategoryDto> { new BookCategoryDto() };

		// Act
		_rankAndOrderService.UpdateBooksRank(bookCategories);

		// Assert
		_mockRankingService.Verify(rs => rs.CalculateBookRank(It.IsAny<List<BookDto>>()), Times.Never());
	}

	[Fact]
	public void CallCalculateBookRankForEachCategoryGivenMultipleCategories()
	{
		// Arrange
		var bookCategories = GetTestBookCategories();
		bookCategories.Add(GetTestBookCategories().First());

		// Act
		_rankAndOrderService.UpdateBooksRank(bookCategories);

		// Assert
		_mockRankingService.Verify(rs => rs.CalculateBookRank(It.IsAny<List<BookDto>>()), Times.Exactly(bookCategories.Count));
	}

	[Fact]
	public void ThrowExceptionGivenNullBookCategories()
	{
		// Arrange
		List<BookCategoryDto> bookCategories = null!;

		// Act & Assert
		Assert.Throws<NullReferenceException>(() => _rankAndOrderService.UpdateBooksRank(bookCategories));
	}

	[Fact]
	public void NotCallCalculateBookRankGivenEmptyBookCategories()
	{
		// Arrange
		var bookCategories = new List<BookCategoryDto>();

		// Act
		_rankAndOrderService.UpdateBooksRank(bookCategories);

		// Assert
		_mockRankingService.Verify(rs => rs.CalculateBookRank(It.IsAny<List<BookDto>>()), Times.Never());
	}

	[Fact]
	public void UpdateBooksRankGivenCategoriesWithMultipleBooks()
	{
		// Arrange
		var bookCategories = GetTestBookCategories();
		bookCategories.First().Books.Add(new BookDto
		{
			Id = 2,
			MembersWhoHaveRead = new List<MemberForBookDto>
			{
				new MemberForBookDto { Id = 4, RoleName = AuthConstants.Roles.MEMBERS, UserId = "4", FullName = "Member2" },
				new MemberForBookDto { Id = 5, RoleName = AuthConstants.Roles.ALUMNI, UserId = "5", FullName = "Alumni3" }
			}
		});

		_mockRankingService.Setup(rs => rs.CalculateBookRank(It.IsAny<List<BookDto>>()))
			.Callback((List<BookDto> books) =>
			{
				foreach (var book in books)
				{
					book.Rank = book.Id!.Value;
				}
			});

		// Act
		_rankAndOrderService.UpdateBooksRank(bookCategories);

		// Assert
		_mockRankingService.Verify(rs => rs.CalculateBookRank(It.IsAny<List<BookDto>>()), Times.Exactly(bookCategories.Count));
		Assert.Equal(1, bookCategories.First().Books.First().Rank);
		Assert.Equal(2, bookCategories.First().Books.Last().Rank);
	}

}

