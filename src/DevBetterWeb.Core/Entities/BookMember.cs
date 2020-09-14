using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities
{
    public class BookMember : BaseEntity
    {
        public Book? Book { get; set; }
        public Member? Member { get; set; }


    }
}
