using DevBetterWeb.Core.Entities;
using System;
using System.Linq;
using Xunit;

namespace DevBetterWeb.Tests.Core.Entities.ArchiveVideoTests
{
    public class ArchiveVideoAddQuestion
    {
        [Fact]
        public void ThrowsArgumentNullExceptionGivenNullQuestion()
        {
            var video = new ArchiveVideo();

            var exception = Assert.Throws<ArgumentNullException>(() => video.AddQuestion(null!));
        }

        [Fact]
        public void AddsQuestionToQuestionsList()
        {
            var video = new ArchiveVideo();
            var question = new Question();

            video.AddQuestion(question);

            Assert.Same(question, video.Questions.First());
        }
    }
}
