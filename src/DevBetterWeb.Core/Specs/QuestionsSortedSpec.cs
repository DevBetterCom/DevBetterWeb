using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;
public sealed class QuestionsSortedSpec : Specification<Question>
{
  public QuestionsSortedSpec()
  {
    Query
	    .OrderByDescending(x => x.CoachingSession!.StartAt);
  }
}
