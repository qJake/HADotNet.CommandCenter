using HADotNet.CommandCenter.Models.Config.Tiles;
using HADotNet.Core.Models;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Hubs
{
    public interface ITileHub
    {
        Task SendTileState(BaseTile tile, StateObject state);

        Task SendDateTime(BaseTile tile, string date, string time);

        Task SendWarning(string message);

        Task RequestTileState(string tileName);
    }
}
