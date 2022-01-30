using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces;

public interface IVideosThumbnailService
{ 
  Task UpdateVideosThumbnail(AppendOnlyStringList messages);
}
