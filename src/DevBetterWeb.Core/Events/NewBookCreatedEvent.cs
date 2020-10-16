using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events
{
    public class NewBookCreatedEvent : BaseDomainEvent
    {
        public NewBookCreatedEvent(Book book)
        {
            Book = book;
        }

        public Book Book { get; }
    }
}
