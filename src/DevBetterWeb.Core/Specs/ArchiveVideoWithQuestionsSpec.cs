using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs
{
    public class ArchiveVideoWithQuestionsSpec : BaseSpecification<ArchiveVideo>
    {
        public ArchiveVideoWithQuestionsSpec(int archiveVideoId)
        {
            ArchiveVideoId = archiveVideoId;

            AddCriteria(video => video.Id == archiveVideoId);
            AddInclude(video => video.Questions);
        }

        public int ArchiveVideoId { get; }
    }
}
