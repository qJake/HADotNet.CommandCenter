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
        public ServiceClient ServiceClient { get; }
        public CalendarClient CalendarClient { get; }
        public IConfigStore ConfigStore { get; }

        public TileHub(StatesClient statesClient, ServiceClient serviceClient, CalendarClient calendarClient, IConfigStore configStore)
        {
            StatesClient = statesClient;
            ServiceClient = serviceClient;
            ConfigStore = configStore;
            CalendarClient = calendarClient;
        }

        public async Task RequestTileState(string page, string tileName)
        {
            var config = await ConfigStore.GetConfigAsync();

            var tile = config[page].Tiles.FirstOrDefault(t => t.Name == tileName);

            // Contains special tiles and overrides that require server-side data(not obtainable directly from HA directly)
            await ProcessOverrides(tile);
        }

        public async Task RequestConfig(string page, string tileName)
        {
            var config = await ConfigStore.GetConfigAsync();
            await Clients.All.SendSystemConfig(tileName, new { BaseUrl = config.Settings.IsHassIo ? config.Settings.ExternalBaseUri : config.Settings.BaseUri });

            var tile = config[page].Tiles.FirstOrDefault(t => t.Name == tileName);
            await Clients.All.SendTile(tile);
        }

        public async Task OnTileClicked(string page, string tileName)
        {
            var config = await ConfigStore.GetConfigAsync();

            var tile = config[page].Tiles.FirstOrDefault(t => t.Name == tileName);

            switch (tile)
            {
                case BaseEntityTile et:
                    await et.OnClick(ServiceClient);
                    break;
                default:
                    break;
            }
        }

        private async Task ProcessOverrides(BaseTile tile)
        {
            switch (tile)
            {
                // Calendars use a special API that isn't (might not be?) exposed via the WebSocket API.
                case CalendarTile ct:
                    await Clients.All.SendCalendarInfo(ct, await StatesClient.GetState(ct.EntityId), await CalendarClient.GetEvents(ct.EntityId));
                    break;

                // Date and time are rendered server-side to verify server connection, and to enforce timezone and date format selection.
                case DateTile dt:
                    var date = !string.IsNullOrWhiteSpace(dt.TimeZoneId)
                        ? TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(dt.TimeZoneId))
                        : DateTime.Now;
                    await Clients.All.SendDateTime(dt, date.ToString(dt.DateFormatString ?? "dddd MMMM d"), date.ToString(dt.TimeFormatString ?? "h:mm tt"));
                    break;

            }
        }
    }
}
