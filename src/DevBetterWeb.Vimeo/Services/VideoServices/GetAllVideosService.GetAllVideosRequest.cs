namespace DevBetterWeb.Vimeo.Services.VideoServices
{
  public class GetAllVideosRequest
  {
    public string UserId { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }

    public GetAllVideosRequest(string userId, int? page=null, int? pageSize=null)
    {
      UserId = userId;
      Page = page;
      PageSize = pageSize;
    }
  }
}
