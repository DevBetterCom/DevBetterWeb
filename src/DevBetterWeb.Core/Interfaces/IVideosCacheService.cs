using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Vimeo.Models;

namespace DevBetterWeb.Core.Interfaces;

public interface IVideosCacheService
{
	Task UpdateAllVideosAsync();
	List<Video> GetAllVideos();
}
