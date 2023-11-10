using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces;

public interface IMapCoordinateService
{
  Task<string> GetMapCoordinates(string address);
}
