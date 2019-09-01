using HADotNet.CommandCenter.Models.Config.Tiles;
using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.Core.Clients;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Controllers
{
    [Route("admin/tile")]
    public class DateTileController : BaseTileController
    {
        public IConfigStore ConfigStore { get; }
        public EntityClient EntityClient { get; }

        public DateTileController(IConfigStore configStore, EntityClient entityClient)
        {
            ConfigStore = configStore;
            EntityClient = entityClient;
        }

        [Route("add/date")]
        public IActionResult Add() => View();

        [Route("edit/date")]
        public async Task<IActionResult> Edit([FromQuery] string name)
        {
            var config = await ConfigStore.GetConfigAsync();

            var tile = config.Tiles.FirstOrDefault(t => t.Name == name);

            return View("Add", tile);
        }

        [HttpPost("add/date")]
        public async Task<IActionResult> Save(DateTile tile)
        {
            if (ModelState.IsValid)
            {
                // Convert empty to null, empty strings cause the full date/time to print.
                if (tile.DateFormatString == "")
                {
                    tile.DateFormatString = null;
                }
                if (tile.TimeFormatString == "")
                {
                    tile.TimeFormatString = null;
                }

                return await SaveBaseTile(ConfigStore, tile);
            }

            return View("Add", tile);
        }
    }
}
