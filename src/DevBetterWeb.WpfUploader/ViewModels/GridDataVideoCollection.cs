using System.Collections.Generic;
using System.Security.Cryptography.Xml;

namespace DevBetterWeb.WpfUploader.ViewModels;

public class GridDataVideoCollection
{
  public static List<GridDataVideo> FromVimeoVideo(List<Vimeo.Models.Video> archiveVideos)
  {
    var result = new List<GridDataVideo>();

    var cnt = 0;
    foreach (var archiveVideo in archiveVideos)
    {
      if (archiveVideo.CreatedTime.Year < 1960)
      {
        continue;
      }

      
      result.Add(new GridDataVideo
      {
        RowNumber = (++cnt).ToString(),
        Title = archiveVideo.Name,
        VideoId = archiveVideo.Id,
        Description = archiveVideo.Description,
        Duration = archiveVideo.Duration,
        DateCreated = archiveVideo.CreatedTime.ToLongDateString(),
        VideoUrl = archiveVideo.Link,
        Status = archiveVideo.Status,
      });
    }

    return result;
  }

  public static List<Vimeo.Models.Video> ToVimeoVideo(List<GridDataVideo> gridDataVideos)
  {
    var result = new List<Vimeo.Models.Video>();

    var cnt = 0;
    foreach (var video in gridDataVideos)
    {
      result.Add(new Vimeo.Models.Video
      {
        Name =  video.Title,
        Description = video.Description,
        Duration = video.Duration,
        Link = video.VideoUrl,
        Status = video.Status,
      });
    }

    return result;
  }
}
