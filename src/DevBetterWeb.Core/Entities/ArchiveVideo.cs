using CleanArchitecture.Core.SharedKernel;
using System;
using System.Collections.Generic;

namespace CleanArchitecture.Core.Entities
{
    public class ArchiveVideo : BaseEntity
    {
        public string Title { get; set; }
        public DateTimeOffset DateCreated { get; set; }

        public string VideoUrl { get; set; }
        public List<Question> Questions { get; private set; } = new List<Question>();
    }
}
