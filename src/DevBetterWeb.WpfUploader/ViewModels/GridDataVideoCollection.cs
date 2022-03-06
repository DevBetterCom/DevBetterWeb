using System.Collections.Generic;

namespace DevBetterWeb.WpfUploader.ViewModels;

public class GridDataVideoCollection
{
  public static List<GridDataVideo> FromVimeoVideo(List<Vimeo.Models.Video> archiveVideos)
  {
    var result = new List<GridDataVideo>();

    foreach (var archiveVideo in archiveVideos)
    {
      if (archiveVideo.CreatedTime.Year < 1960)
      {
        continue;
      }
      result.Add(new GridDataVideo
      {
        Title = archiveVideo.Name,
        VideoId = archiveVideo.Id,
        Description = archiveVideo.Description,
        Duration = archiveVideo.Duration,
        DateCreated = archiveVideo.CreatedTime,
        DateUploaded = archiveVideo.CreatedTime,
        VideoUrl = archiveVideo.Link,
        Status = archiveVideo.Status,
      });
    }

    return result;
  }
}
