using System;
using System.Linq;
using DevBetterWeb.Core.Entities;
using Xunit;

namespace DevBetterWeb.UnitTests.Core.ArchiveVideoTests;

public class ArchiveVideoAddQuestion
{
  [Fact]
  public void ThrowsArgumentNullExceptionGivenNullQuestion()
  {
    var video = new ArchiveVideo();

    var exception = Assert.Throws<ArgumentNullException>(() => video.AddQuestion(null!));
  }
}
