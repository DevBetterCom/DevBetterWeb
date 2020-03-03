using DevBetterWeb.Core.Entities;
using System;
using Xunit;

namespace DevBetterWeb.Tests.Core.Entities.ArchiveVideoTests
{
    public class ArchiveVideoNew
    {
        [Fact]
        public void InitializesListOfQuestions()
        {
            var video = new ArchiveVideo();

            Assert.NotNull(video.Questions);
        }

        [Theory]
        [InlineData(2019,5,17, "2019-05-17")]
        public void DateStringIsExpectedValue(int year, int month, int day, string output)
        {
            var inputDateTime = new DateTime(year, month, day);

            string result = inputDateTime.ToString("yyyy-MM-dd");

            Assert.Equal(output, result);
        }
    }
}
