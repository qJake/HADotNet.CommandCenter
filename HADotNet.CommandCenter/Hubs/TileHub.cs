using HADotNet.CommandCenter.Models.Config.Tiles;
using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.Core.Clients;
using Microsoft.AspNetCore.SignalR;
using System;
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

            switch (tile)
            {
                case BaseEntityTile et:
                    var state = await StatesClient.GetState(et.EntityId);
                    await Clients.All.SendTileState(et, state);
                    break;
                case DateTile dt:
                    await Clients.All.SendDateTime(dt, DateTime.Now.ToString("dddd MMMM d"), DateTime.Now.ToString("h:mm tt"));
                    break;
                case BlankTile _:
                case null:
                    break;
                default:
                    await Clients.All.SendWarning($"Tile of type {tile.GetType().FullName} does not have a send method.");
                    break;
            }

        }
    }
}
