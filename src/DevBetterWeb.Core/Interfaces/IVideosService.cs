using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces;

public interface IVideosService
{ 
  Task UpdateVideosThumbnail(AppendOnlyStringList? messages);
  Task DeleteVideosNotExistOnVimeoFromDatabase(AppendOnlyStringList? messages);
  Task DeleteVideosNotExistOnVimeoFromVimeo(AppendOnlyStringList? messages);
}
