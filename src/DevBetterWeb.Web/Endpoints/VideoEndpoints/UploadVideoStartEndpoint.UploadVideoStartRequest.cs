using System;

namespace DevBetterWeb.Web.Endpoints.VideoEndpoints;

public class UploadVideoStartRequest
{
	public string VideoName { get; set; } = string.Empty;
	public long VideoSize { get; set; }
	public DateTimeOffset CreatedTime { get; set; }
}
