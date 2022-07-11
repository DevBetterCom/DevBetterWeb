using System;
using System.Collections.Generic;

namespace DevBetterWeb.Web.Models;

public class VideoCommentDto
{
	public int Id { get; set; }
	public int VideoId { get; set; }
	public string? Body { get; set; }
	public string? MdBody { get; set; }
	public int MemberId { get; set; }
	public string MemberName { get; set; }
	public DateTimeOffset CreatedAt { get; set; }
	public List<VideoCommentDto> Replies { get; private set; } = new List<VideoCommentDto>();
}
