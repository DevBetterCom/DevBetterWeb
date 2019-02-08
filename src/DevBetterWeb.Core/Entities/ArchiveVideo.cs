using CleanArchitecture.Core.SharedKernel;
using System;

namespace CleanArchitecture.Core.Entities
{
    public class ArchiveVideo : BaseEntity
    {
        public string Title { get; set; }
        public DateTimeOffset DateCreated { get; set; }

        public string VideoUrl { get; set; }
    }
}
