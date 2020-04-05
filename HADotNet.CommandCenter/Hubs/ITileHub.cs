using HADotNet.CommandCenter.Models.Config.Tiles;
using HADotNet.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Hubs
{
    public interface ITileHub
    {
        Task SendTile(BaseTile tile);

        Task SendTileState(BaseTile tile, StateObject state);


        Task SendTileStates(BaseTile tile, Dictionary<string, StateObject> states);

        Task SendDateTime(BaseTile tile, string date, string time);

        Task SendCalendarInfo(BaseTile tile, StateObject state, List<CalendarObject> events);

        Task SendWarning(string message);

        Task RequestTileState(string page, string tileName);

        Task SendSystemConfig(string tileName, object config);
    }
}
