using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public sealed class ArchiveVideoWithQuestionsSpec : Specification<ArchiveVideo>, 
	ISingleResultSpecification<ArchiveVideo>
{
  public ArchiveVideoWithQuestionsSpec(int archiveVideoId)
  {
	  Query
	    .Where(video => video.Id == archiveVideoId);
  }
}
