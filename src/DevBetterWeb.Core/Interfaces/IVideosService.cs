using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces;

public interface IVideosService
{ 
  Task UpdateVideosThumbnail(AppendOnlyStringList messages);
  Task UpdateVideosThumbnailWithoutMessages();
  Task DeleteVideosNotExistOnVimeo(AppendOnlyStringList messages);
  Task DeleteVideosNotExistOnVimeoWithoutMessages();
}
