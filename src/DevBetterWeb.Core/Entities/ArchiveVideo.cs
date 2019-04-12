using CleanArchitecture.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Core.Entities
{
    public class ArchiveVideo : BaseEntity
    {
        [Required]
        public string Title { get; set; }
        [DisplayName("Show Notes")]
        public string ShowNotes { get; set; }

        [DisplayName("Date Created")]
        public DateTimeOffset DateCreated { get; set; }

        [DisplayName("Video URL")]
        public string VideoUrl { get; set; }
        public List<Question> Questions { get; private set; } = new List<Question>();

        public void AddQuestion(Question question)
        {
            // TODO: Add Guard Clause
            if(question != null)
            {
                Questions.Add(question);
            }
        }
    }
}
