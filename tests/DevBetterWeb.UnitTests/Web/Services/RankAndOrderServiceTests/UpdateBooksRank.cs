using System;
using System.Collections.Generic;
using DevBetterWeb.Core;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Services;
using Xunit;
using NSubstitute;

namespace DevBetterWeb.UnitTests.Web.Services.RankAndOrderServiceTests;

public class UpdateBooksRank
{
	private readonly IRankingService _mockRankingService;
	private readonly RankAndOrderService _rankAndOrderService;

	public UpdateBooksRank()
	{
		_mockRankingService = Substitute.For<IRankingService>();
		var mockMemberService = Substitute.For<IMemberService>();
		_rankAndOrderService = new RankAndOrderService(_mockRankingService, mockMemberService);
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
							new MemberForBookDto
							{
								Id = 1, RoleName = AuthConstants.Roles.ALUMNI, UserId = "1", FullName = "Alumni1"
							},
							new MemberForBookDto
							{
								Id = 2, RoleName = AuthConstants.Roles.MEMBERS, UserId = "2", FullName = "Member1"
							},
							new MemberForBookDto
							{
								Id = 3, RoleName = AuthConstants.Roles.ALUMNI, UserId = "3", FullName = "Alumni2"
							}
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
		_mockRankingService.Received(bookCategories.Count).CalculateBookRank(Arg.Any<List<BookDto>>());
	}

	[Fact]
	public void NotCallCalculateBookRankGivenCategoryWithoutBooks()
	{
		// Arrange
		var bookCategories = new List<BookCategoryDto> { new BookCategoryDto() };

		// Act
		_rankAndOrderService.UpdateBooksRank(bookCategories);

		// Assert
		_mockRankingService.DidNotReceive().CalculateBookRank(Arg.Any<List<BookDto>>());
	}

	[Fact]
	public void CallCalculateBookRankForEachCategoryGivenMultipleCategories()
	{
		// Arrange
		var bookCategories = GetTestBookCategories();
		bookCategories.Add(GetTestBookCategories()[0]);

		// Act
		_rankAndOrderService.UpdateBooksRank(bookCategories);

		// Assert
		_mockRankingService.Received(bookCategories.Count).CalculateBookRank(Arg.Any<List<BookDto>>());
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
		_mockRankingService.DidNotReceive().CalculateBookRank(Arg.Any<List<BookDto>>());
	}

	[Fact]
	public void UpdateBooksRankGivenCategoriesWithMultipleBooks()
	{
		// Arrange
		var bookCategories = GetTestBookCategories();
		bookCategories[0].Books.Add(new BookDto
		{
			Id = 2,
			MembersWhoHaveRead = new List<MemberForBookDto>
			{
				new MemberForBookDto { Id = 4, RoleName = AuthConstants.Roles.MEMBERS, UserId = "4", FullName = "Member2" },
				new MemberForBookDto { Id = 5, RoleName = AuthConstants.Roles.ALUMNI, UserId = "5", FullName = "Alumni3" }
			}
		});

		_mockRankingService.CalculateBookRank(Arg.Do<List<BookDto>>(books =>
		{
			foreach (var book in books)
			{
				book.Rank = book.Id!.Value;
			}
		}));

		// Act
		_rankAndOrderService.UpdateBooksRank(bookCategories);

		// Assert
		_mockRankingService.Received(bookCategories.Count).CalculateBookRank(Arg.Any<List<BookDto>>());
		Assert.Equal(1, bookCategories[0].Books[0].Rank);
		Assert.Equal(2, bookCategories[0].Books[1].Rank);
	}
}
