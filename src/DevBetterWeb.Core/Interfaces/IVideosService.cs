using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Interfaces;

public interface IVideosService
{ 
  Task UpdateVideosThumbnail(AppendOnlyStringList? messages, CancellationToken cancellationToken = default);
  Task DeleteVideosNotExistOnVimeoFromDatabase(AppendOnlyStringList? messages);
  Task DeleteVideosNotExistOnVimeoFromVimeo(AppendOnlyStringList? messages);
  Task AddArchiveVideoInfo(ArchiveVideo archiveVideo, CancellationToken cancellationToken = default);
  Task<ArchiveVideo?> UpdateVideoThumbnailsAsync(long videoId, CancellationToken cancellationToken = default);
}
