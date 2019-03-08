using CleanArchitecture.Core.SharedKernel;

namespace CleanArchitecture.Core.Entities
{

    public class Question : BaseEntity
    {
        public int ArchiveVideoId { get; set; }
        public string QuestionText { get; set; }
        public int TimestampSeconds { get; set; }
    }
}
