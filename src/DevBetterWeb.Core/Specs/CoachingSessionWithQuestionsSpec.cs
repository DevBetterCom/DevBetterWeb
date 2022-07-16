using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public sealed class CoachingSessionWithQuestionsSpec : Specification<CoachingSession>, ISingleResultSpecification
{
  public CoachingSessionWithQuestionsSpec(int coachingSessionId)
  {
	  Query
		  .Where(coachingSession => coachingSession.Id == coachingSessionId)
		  .Include(coachingSession => coachingSession.Questions)
				.ThenInclude(question => question.QuestionVotes)
		  .OrderByDescending(x => x.StartAt);
  }
}
