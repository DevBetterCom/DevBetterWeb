using Ardalis.GuardClauses;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;
using System;
using System.Collections.Generic;

namespace DevBetterWeb.Core.Entities
{
    public class ArchiveVideo : BaseEntity, IAggregateRoot
    {
        public string? Title { get; set; }
        public string? ShowNotes { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public string? VideoUrl { get; set; }

        public List<Question> Questions { get; private set; } = new List<Question>();

        public void AddQuestion(Question question)
        {
            Guard.Against.Null(question, nameof(question));
            Questions.Add(question);
        }
    }
}
