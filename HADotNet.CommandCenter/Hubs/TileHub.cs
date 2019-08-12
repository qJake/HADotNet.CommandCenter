using HADotNet.CommandCenter.Models.Config.Tiles;
using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.Core;
using HADotNet.Core.Clients;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Hubs
{
    public class TileHub : Hub<ITileHub>
    {
        private StatesClient StatesClient { get; }
        public IConfigStore ConfigStore { get; }

        public TileHub(StatesClient statesClient, IConfigStore configStore)
        {
            StatesClient = statesClient;
            ConfigStore = configStore;
        }

        public async Task RequestTileState(string tileName)
        {
            var config = await ConfigStore.GetConfigAsync();

            var tile = config.Tiles.FirstOrDefault(t => t.Name == tileName);

            if (tile != null && tile is BaseEntityTile et)
            {
                var state = await StatesClient.GetState(et.EntityId);

                await Clients.All.SendTileState(et, state);
            }
        }
    }
}
