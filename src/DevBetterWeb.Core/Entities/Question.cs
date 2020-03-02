using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities
{
    public class Question : BaseEntity
    {
        public int ArchiveVideoId { get; set; }
        public string? QuestionText { get; set; }
        public int TimestampSeconds { get; set; }
    }
}
