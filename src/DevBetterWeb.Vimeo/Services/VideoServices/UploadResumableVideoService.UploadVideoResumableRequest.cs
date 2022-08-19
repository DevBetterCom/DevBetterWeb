namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class UploadVideoResumableInfo
{
	public string VideoId { get; set; } = string.Empty;
	public string VideoUrl { get; set; } = string.Empty;
	public byte[] FilePartData { get; set; }
	public int FileFullSize { get; set; }
	public string UploadUrl { get; set; }
	public string FileName { get; set; }
	public int UploadOffset { get; set; }
	public int PartSize { get; set; }
}
