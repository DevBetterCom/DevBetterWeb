using System.Collections.Generic;
using DevBetterWeb.Core;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Services;
using Xunit;
using Moq;
using System.Linq;

namespace DevBetterWeb.UnitTests.Web.Services.RankAndOrderServiceTests;

public class UpdateMembersReadRankTests
{
	private readonly Mock<IRankingService> _mockRankingService;
	private readonly RankAndOrderService _rankAndOrderService;

	public UpdateMembersReadRankTests()
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
	public void UpdatesRanksForMembersAndAlumni()
	{
		// Arrange
		var bookCategories = GetTestBookCategories();

		// Act
		_rankAndOrderService.UpdateMembersReadRank(bookCategories);

		// Assert
		foreach (var category in bookCategories)
		{
			_mockRankingService.Verify(rs => rs.CalculateMemberRank(category.Members), Times.AtLeastOnce());
			_mockRankingService.Verify(rs => rs.CalculateMemberRank(category.Alumnus), Times.AtLeastOnce());
		}
	}

	[Fact]
	public void UpdatesRanksForMembersAndAlumni_NoBooksInCategory_DoesNotCallCalculateMemberRank()
	{
		// Arrange
		var bookCategories = new List<BookCategoryDto> { new BookCategoryDto() };

		// Act
		_rankAndOrderService.UpdateMembersReadRank(bookCategories);

		// Assert
		_mockRankingService.Verify(rs => rs.CalculateMemberRank(It.IsAny<List<MemberForBookDto>>()), Times.Never());
	}


	[Fact]
	public void UpdatesRanksForMembersAndAlumni_MultipleCategories_CallsCalculateMemberRankForEachCategory()
	{
		// Arrange
		var bookCategories = GetTestBookCategories();
		bookCategories.Add(GetTestBookCategories().First());

		// Act
		_rankAndOrderService.UpdateMembersReadRank(bookCategories);

		// Assert
		_mockRankingService.Verify(rs => rs.CalculateMemberRank(It.IsAny<List<MemberForBookDto>>()), Times.Exactly(bookCategories.Count * 2));
	}
}
