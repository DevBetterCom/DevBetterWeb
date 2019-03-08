using CleanArchitecture.Core.Entities;
using System.Linq;
using Xunit;

namespace CleanArchitecture.Tests.Core.Entities
{
    public class ArchiveVideoAddQuestion
    {
        [Fact]
        public void DoesNothingGivenNullQuestion()
        {
            var video = new ArchiveVideo();

            video.AddQuestion(null);

            Assert.Empty(video.Questions);
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
