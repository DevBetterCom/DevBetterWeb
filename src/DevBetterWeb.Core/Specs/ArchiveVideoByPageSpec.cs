using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs
{
    public class ArchiveVideoByPageSpec : Specification<ArchiveVideo>, ISingleResultSpecification
    {
        public ArchiveVideoByPageSpec(int skip, int size)
        {
            Query.OrderByDescending(x => x.DateCreated).Skip(skip).Take(size);
        }
    }
}
