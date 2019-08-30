using HADotNet.CommandCenter.Models.Config.Tiles;
using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.Core.Clients;
using HADotNet.Core.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Hubs
{
    public class TileHub : Hub<ITileHub>
    {
        private StatesClient StatesClient { get; }
        public ServiceClient ServiceClient { get; }
        public IConfigStore ConfigStore { get; }

        public TileHub(StatesClient statesClient, ServiceClient serviceClient, IConfigStore configStore)
        {
            StatesClient = statesClient;
            ServiceClient = serviceClient;
            ConfigStore = configStore;
        }

        public async Task RequestTileState(string tileName)
        {
            var config = await ConfigStore.GetConfigAsync();

            var tile = config.Tiles.FirstOrDefault(t => t.Name == tileName);

            // TOOD: Refactor this into something more elegant.
            switch (tile)
            {
                case WeatherTile wt:
                    var states = new Dictionary<string, StateObject> {
                        [nameof(wt.EntityId)] = wt.StateManipulator(await StatesClient.GetState(wt.EntityId)),
                        [nameof(wt.IconEntity)] = string.IsNullOrWhiteSpace(wt.IconEntity) ? null : wt.StateManipulator(await StatesClient.GetState(wt.IconEntity)),
                        [nameof(wt.SummaryEntity)] = string.IsNullOrWhiteSpace(wt.SummaryEntity) ? null : wt.StateManipulator(await StatesClient.GetState(wt.SummaryEntity)),
                        [nameof(wt.PrecipChanceEntity)] = string.IsNullOrWhiteSpace(wt.PrecipChanceEntity) ? null : wt.StateManipulator(await StatesClient.GetState(wt.PrecipChanceEntity)),
                        [nameof(wt.HighTempEntity)] = string.IsNullOrWhiteSpace(wt.HighTempEntity) ? null : wt.StateManipulator(await StatesClient.GetState(wt.HighTempEntity)),
                        [nameof(wt.LowTempEntity)] = string.IsNullOrWhiteSpace(wt.LowTempEntity) ? null : wt.StateManipulator(await StatesClient.GetState(wt.LowTempEntity)),
                        [nameof(wt.WindSpeedEntity)] = string.IsNullOrWhiteSpace(wt.WindSpeedEntity) ? null : wt.StateManipulator(await StatesClient.GetState(wt.WindSpeedEntity)),
                        [nameof(wt.WindDirectionEntity)] = string.IsNullOrWhiteSpace(wt.WindDirectionEntity) ? null : wt.StateManipulator(await StatesClient.GetState(wt.WindDirectionEntity))
                    };
                    await Clients.All.SendTileStates(wt, states);
                    break;
                case BaseEntityTile et:
                    var state = await StatesClient.GetState(et.EntityId);
                    state = et.StateManipulator(state);
                    await Clients.All.SendTileState(et, state);
                    break;
                case DateTile dt:
                    await Clients.All.SendDateTime(dt, DateTime.Now.ToString("dddd MMMM d"), DateTime.Now.ToString("h:mm tt"));
                    break;
                case BlankTile _:
                case LabelTile _:
                case null:
                    break;
                default:
                    await Clients.All.SendWarning($"Tile of type {tile.GetType().FullName} does not have a send method.");
                    break;
            }

        }

        public async Task RequestConfig(string tileName)
        {
            var config = await ConfigStore.GetConfigAsync();
            await Clients.All.SendSystemConfig(tileName, new { BaseUrl = config.Settings.BaseUri });
        }

        public async Task OnTileClicked(string tileName)
        {
            var config = await ConfigStore.GetConfigAsync();

            var tile = config.Tiles.FirstOrDefault(t => t.Name == tileName);

            switch (tile)
            {
                case BaseEntityTile et:
                    await et.OnClick(ServiceClient);
                    break;
                default:
                    break;
            }
        }
    }
}
