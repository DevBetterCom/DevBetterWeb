using CleanArchitecture.Core.Entities;
using CleanArchitecture.Core.Events;
using System.Linq;
using Xunit;

namespace CleanArchitecture.Tests.Core.Entities
{
    public class ArchiveVideoNew
    {
        [Fact]
        public void InitializesListOfQuestions()
        {
            var video = new ArchiveVideo();

            Assert.NotNull(video.Questions);
        }
    }
}
