using System.Collections.Generic;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities;

public class VideoComment : BaseEntity, IAggregateRoot
{
  public string? VideoId { get; set; }
  public string? ParentCommentId { get; set; }
  public string? Body { get; set; }
  public int MemberId { get; set; }
  public Member MemberWhoCreate { get; set; } = new Member();
  public VideoComment ParentComment { get; set; } = new VideoComment();
  public List<ArchiveVideo> Videos { get; set; } = new List<ArchiveVideo>();

  public override string ToString()
  {
    return Body!;
  }
}
