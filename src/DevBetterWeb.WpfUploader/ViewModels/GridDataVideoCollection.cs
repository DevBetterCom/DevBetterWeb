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
}
