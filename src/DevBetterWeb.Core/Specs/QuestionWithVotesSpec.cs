using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public sealed class QuestionWithVotesSpec : Specification<Question>, 
	ISingleResultSpecification<Question>
{
  public QuestionWithVotesSpec(int questionId)
  {
	  Query
		  .Where(question => question.Id == questionId)
		  .Include(question => question.QuestionVotes);
  }
}
