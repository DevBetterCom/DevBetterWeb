using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public sealed class CoachingSessionsWithQuestionsSpec : Specification<CoachingSession>
{
  public CoachingSessionsWithQuestionsSpec(int? recentCount=null)
  {
	  Query
		  .OrderByDescending(x => x.StartAt);


	  if (recentCount != null)
	  {
		  Query
			  .Take(recentCount.Value);
		}

	  Query
		  .Include(coachingSession => coachingSession.Questions)
				.ThenInclude(question => question.QuestionVotes);
  }
}
