using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities
{
    public class BookMember : BaseEntity
    {
        public Book Book { get; set; } = new Book();
        public Member Member { get; set; } = new Member(string.Empty);

    }
}
