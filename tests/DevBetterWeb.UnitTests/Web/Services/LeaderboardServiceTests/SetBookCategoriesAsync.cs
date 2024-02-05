using System.Collections.Generic;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Services;
using Xunit;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using NSubstitute.ExceptionExtensions;

namespace DevBetterWeb.UnitTests.Web.Services.LeaderboardServiceTests;

    public class SetBookCategoriesAsync
    {
        private readonly IRankAndOrderService _rankAndOrderServiceMock;
        private readonly IBookCategoryService _bookCategoryServiceMock;
        private readonly IFilteredLeaderboardService _filteredLeaderboardServiceMock;
        private readonly LeaderboardService _leaderboardService;

        public SetBookCategoriesAsync()
        {
            _rankAndOrderServiceMock = Substitute.For<IRankAndOrderService>();
            _bookCategoryServiceMock = Substitute.For<IBookCategoryService>();
            _filteredLeaderboardServiceMock = Substitute.For<IFilteredLeaderboardService>();
            _leaderboardService = new LeaderboardService(_rankAndOrderServiceMock, _bookCategoryServiceMock, _filteredLeaderboardServiceMock);
        }

        [Fact]
        public async Task UpdateRanksAndOrdersGivenBookCategories()
        {
            // Arrange
            var bookCategories = new List<BookCategoryDto>
            {
                new BookCategoryDto
                {
                    Members = new List<MemberForBookDto>
                    {
                        new MemberForBookDto { Id = 1, UserId = "UserId1" },
                        new MemberForBookDto { Id = 2, UserId = "UserId2" }
                    }
                }
            };

            _bookCategoryServiceMock.GetBookCategoriesAsync().Returns(bookCategories);
            _filteredLeaderboardServiceMock.RemoveNonCurrentMembersFromLeaderBoardAsync(bookCategories, CancellationToken.None).Returns(bookCategories);

            // Act
            await _leaderboardService.SetBookCategoriesAsync();

            // Assert
            _rankAndOrderServiceMock.Received(1).UpdateMembersReadRank(Arg.Any<List<BookCategoryDto>>());
            _rankAndOrderServiceMock.Received(1).UpdateBooksRank(Arg.Any<List<BookCategoryDto>>());
            _rankAndOrderServiceMock.Received(1).OrderByRankForMembersAndBooks(Arg.Any<List<BookCategoryDto>>());
           await _rankAndOrderServiceMock.DidNotReceiveWithAnyArgs().UpdateRanksAndReadBooksCountForMemberAsync(Arg.Any<List<BookCategoryDto>>());
        }

        [Fact]
        public async Task NotIncludeNonActiveMemberInRankGivenNonActiveMember()
        {
            // Arrange
            var bookCategories = new List<BookCategoryDto>
            {
                new BookCategoryDto
                {
                    Members = new List<MemberForBookDto>
                    {
                        new MemberForBookDto { Id = 1, UserId = "UserId1" },
                        new MemberForBookDto { Id = 2, UserId = "UserId2" }
                    }
                }
            };
            var bookCategoriesWithoutNonActiveMember = new List<BookCategoryDto>
            {
                new BookCategoryDto
                {
                    Members = new List<MemberForBookDto>
                    {
                        new MemberForBookDto { Id = 2, UserId = "UserId2" }
                    }
                }
            };

            _bookCategoryServiceMock.GetBookCategoriesAsync().Returns(bookCategories);
            _filteredLeaderboardServiceMock.RemoveNonCurrentMembersFromLeaderBoardAsync(bookCategories, CancellationToken.None).Returns(bookCategoriesWithoutNonActiveMember);

            // Act
            var result = await _leaderboardService.SetBookCategoriesAsync();

            // Assert
            Assert.Single(result[0].Members);
            Assert.Equal(2, result[0].Members[0].Id);
        }

        [Fact]
        public async Task ReturnsEmptyListGivenNoBookCategories()
        {
            // Arrange
            var bookCategories = new List<BookCategoryDto>();

            _bookCategoryServiceMock.GetBookCategoriesAsync().Returns(bookCategories);
            _filteredLeaderboardServiceMock.RemoveNonCurrentMembersFromLeaderBoardAsync(bookCategories, CancellationToken.None).Returns(bookCategories);

            // Act
            var result = await _leaderboardService.SetBookCategoriesAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task ThrowsExceptionGivenGetBookCategoriesAsyncFails()
        {
            // Arrange
            _bookCategoryServiceMock.GetBookCategoriesAsync().Throws(new ArgumentNullException());

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _leaderboardService.SetBookCategoriesAsync());
        }

        [Fact]
        public async Task OrdersMemberRanksCorrectlyGivenUnorderedRanks()
        {
            // Arrange
            var bookCategories = new List<BookCategoryDto>
            {
                new BookCategoryDto
                {
                    Members = new List<MemberForBookDto>
                    {
                        new MemberForBookDto { Id = 1, UserId = "UserId1", BooksRank = 2},
                        new MemberForBookDto { Id = 2, UserId = "UserId2", BooksRank = 1 }
                    }
                }
            };

            _bookCategoryServiceMock.GetBookCategoriesAsync().Returns(bookCategories);
            _filteredLeaderboardServiceMock.RemoveNonCurrentMembersFromLeaderBoardAsync(bookCategories, CancellationToken.None).Returns(bookCategories);

            _rankAndOrderServiceMock.OrderByRankForMembersAndBooks(Arg.Do<List<BookCategoryDto>>(categories =>
            {
                foreach (var category in categories)
                {
                    category.Members = category.Members.OrderBy(m => m.BooksRank).ToList();
                }
            }));

            // Act
            var result = await _leaderboardService.SetBookCategoriesAsync();

            // Assert
            Assert.Equal(2, result[0].Members.Count);
            Assert.Equal(1, result[0].Members[0].BooksRank);
            Assert.Equal(2, result[0].Members[1].BooksRank);
        }

        [Fact]
        public async Task OrdersMembersCorrectlyByBooksReadCountGivenUnorderedMembers()
        {
            // Arrange
            var bookCategories = new List<BookCategoryDto>
            {
                new BookCategoryDto
                {
                    Members = new List<MemberForBookDto>
                    {
                        new MemberForBookDto { Id = 1, UserId = "UserId1", BooksReadCount = 1, BooksRank = 2 },
                        new MemberForBookDto { Id = 2, UserId = "UserId2", BooksReadCount = 3, BooksRank = 1 }
                    }
                }
            };

            _bookCategoryServiceMock.GetBookCategoriesAsync().Returns(bookCategories);
            _filteredLeaderboardServiceMock.RemoveNonCurrentMembersFromLeaderBoardAsync(bookCategories, CancellationToken.None).Returns(bookCategories);

            _rankAndOrderServiceMock.OrderByRankForMembersAndBooks(Arg.Do<List<BookCategoryDto>>(categories =>
            {
                foreach (var category in categories)
                {
                    category.Members = category.Members.OrderByDescending(m => m.BooksReadCount).ThenBy(m => m.BooksRank).ToList();
                }
            }));

            // Act
            var result = await _leaderboardService.SetBookCategoriesAsync();

            // Assert
            Assert.Equal(2, result[0].Members.Count);
            Assert.Equal(3, result[0].Members[0].BooksReadCount);
            Assert.Equal(1, result[0].Members[0].BooksRank);
            Assert.Equal(1, result[0].Members[1].BooksReadCount);
            Assert.Equal(2, result[0].Members[1].BooksRank);
        }
    }

