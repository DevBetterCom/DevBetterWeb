using System.Collections.Generic;
using DevBetterWeb.Core;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Services;
using Xunit;
using Moq;
using System;

namespace DevBetterWeb.UnitTests.Web.Services.RankAndOrderServiceTests;

public class OrderByRankForMembersAndBooks
{
	private readonly Mock<IRankingService> _mockRankingService;
	private readonly RankAndOrderService _rankAndOrderService;

	public OrderByRankForMembersAndBooks()
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
	public void OrderMembersAndBooksGivenBookCategories()
	{
		// Arrange
		var bookCategories = GetTestBookCategories();

		// Act
		_rankAndOrderService.OrderByRankForMembersAndBooks(bookCategories);

		// Assert
		_mockRankingService.Verify(rs => rs.OrderMembersByRank(It.IsAny<List<MemberForBookDto>>()), Times.Exactly(bookCategories.Count * 2));
		_mockRankingService.Verify(rs => rs.OrderBooksByRank(It.IsAny<List<BookDto>>()), Times.Exactly(bookCategories.Count));
	}

	[Fact]
	public void NotCallRankingServiceMethodsGivenEmptyList()
	{
		// Arrange
		var bookCategories = new List<BookCategoryDto>();

		// Act
		_rankAndOrderService.OrderByRankForMembersAndBooks(bookCategories);

		// Assert
		_mockRankingService.Verify(rs => rs.OrderMembersByRank(It.IsAny<List<MemberForBookDto>>()), Times.Never());
		_mockRankingService.Verify(rs => rs.OrderBooksByRank(It.IsAny<List<BookDto>>()), Times.Never());
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
