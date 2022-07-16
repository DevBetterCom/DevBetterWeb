using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public sealed class CoachingSessionsWithQuestionsSpec : Specification<CoachingSession>
{
  public CoachingSessionsWithQuestionsSpec()
  {
	  Query
		  .Include(coachingSession => coachingSession.Questions)
				.ThenInclude(question => question.QuestionVotes)
		  .OrderByDescending(x => x.StartAt);
  }
}
