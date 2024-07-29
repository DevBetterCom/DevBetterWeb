using System.Collections.Generic;
using DevBetterWeb.Core;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Services;
using Xunit;
using NSubstitute;
using System;

namespace DevBetterWeb.UnitTests.Web.Services.RankAndOrderServiceTests;

public class OrderByRankForMembersAndBooks
{
	private readonly IRankingService _mockRankingService;
	private readonly RankAndOrderService _rankAndOrderService;

	public OrderByRankForMembersAndBooks()
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
	public void OrderMembersAndBooksGivenBookCategories()
	{
		// Arrange
		var bookCategories = GetTestBookCategories();

		// Act
		_rankAndOrderService.OrderByRankForMembersAndBooks(bookCategories);

		// Assert
		_mockRankingService.Received(bookCategories.Count * 2).OrderMembersByRank(Arg.Any<List<MemberForBookDto>>());
		_mockRankingService.Received(bookCategories.Count).OrderBooksByRank(Arg.Any<List<BookDto>>());
	}

	[Fact]
	public void NotCallRankingServiceMethodsGivenEmptyList()
	{
		// Arrange
		var bookCategories = new List<BookCategoryDto>();

		// Act
		_rankAndOrderService.OrderByRankForMembersAndBooks(bookCategories);

		// Assert
		_mockRankingService.DidNotReceive().OrderMembersByRank(Arg.Any<List<MemberForBookDto>>());
		_mockRankingService.DidNotReceive().OrderBooksByRank(Arg.Any<List<BookDto>>());
	}

	[Fact]
	public void ThrowsNullReferenceExceptionGivenNullList()
	{
		// Arrange
		List<BookCategoryDto> bookCategories = null!;

		// Act & Assert
		Assert.Throws<NullReferenceException>(() => _rankAndOrderService.OrderByRankForMembersAndBooks(bookCategories));
	}

	[Fact]
	public void ThrowsArgumentExceptionGivenListContainsNull()
	{
		// Arrange
		var bookCategories = new List<BookCategoryDto> { null! };

		// Act & Assert
		Assert.Throws<NullReferenceException>(() => _rankAndOrderService.OrderByRankForMembersAndBooks(bookCategories));
	}

}
