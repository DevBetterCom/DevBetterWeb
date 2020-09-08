using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs
{
    public class ArchiveVideoWithQuestionsSpec : Specification<ArchiveVideo>
    {
        public ArchiveVideoWithQuestionsSpec(int archiveVideoId)
        {
            ArchiveVideoId = archiveVideoId;

            Query.Where(video => video.Id == archiveVideoId)
                .Include(video => video.Questions);
        }

        public int ArchiveVideoId { get; }
    }
}
