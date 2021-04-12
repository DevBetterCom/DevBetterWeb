using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IMapCoordinateService
  {
    Task<string> GetMapCoordinates(string address);
  }
}
