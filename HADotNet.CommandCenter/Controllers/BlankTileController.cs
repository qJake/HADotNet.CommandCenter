using HADotNet.CommandCenter.Models.Config.Tiles;
using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.Core.Clients;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Controllers
{
    [Route("admin/pages/{page}/tile")]
    public class BlankTileController : BaseTileController
    {
        public IConfigStore ConfigStore { get; }
        public EntityClient EntityClient { get; }

        public BlankTileController(IConfigStore configStore, EntityClient entityClient)
        {
            ConfigStore = configStore;
            EntityClient = entityClient;
        }

        [Route("add/blank")]
        public IActionResult Add() => View();

        [Route("edit/blank")]
        public async Task<IActionResult> Edit([FromRoute] string page, string name)
        {
            var config = await ConfigStore.GetConfigAsync();

            var tile = config[page].Tiles.FirstOrDefault(t => t.Name == name);

            return View("Add", tile);
        }

        [HttpPost("add/blank")]
        public async Task<IActionResult> Save([FromRoute] string page, BlankTile tile)
        {
            if (ModelState.IsValid)
            {
                return await SaveBaseTile(page, ConfigStore, tile);
            }

            return View("Add", tile);
        }
    }
}
