using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs
{
    public class ArchiveVideoByPageSpec : Specification<ArchiveVideo>, ISingleResultSpecification
    {
        public ArchiveVideoByPageSpec(int page, int size)
        {
            Query.Skip(page*size).Take(size);
        }
    }
}
