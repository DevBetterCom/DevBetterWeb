namespace DevBetterWeb.Web.Endpoints.VideoEndpoints;

public class UploadChunkRequest
{
	public string SessionId { get; set; } = string.Empty;
	public string Chunk { get; set; } = string.Empty;
	public string? Description { get; set; }
	public long? FolderId { get; set; }
}
