using HADotNet.CommandCenter.Models.Config.Tiles;
using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.Core.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Controllers
{
    [Route("admin/tile")]
    public class SwitchTileController : BaseTileController
    {
        public IConfigStore ConfigStore { get; }
        public EntityClient EntityClient { get; }

        public SwitchTileController(IConfigStore configStore, EntityClient entityClient)
        {
            ConfigStore = configStore;
            EntityClient = entityClient;
        }

        private async Task PopulateSelectLists()
        {
            ViewBag.Entities = (await EntityClient.GetEntities("switch"))
                .Union(await EntityClient.GetEntities("group"))
                .Union(await EntityClient.GetEntities("cover"))
                .Union(await EntityClient.GetEntities("input_boolean"))
                .OrderBy(e => e)
                .Select(e => new SelectListItem(e, e));
        }

        [Route("add/switch")]
        public async Task<IActionResult> Add()
        {
            await PopulateSelectLists();
            return View();
        }

        [Route("edit/switch")]
        public async Task<IActionResult> Edit([FromQuery] string name)
        {
            await PopulateSelectLists();

            var config = await ConfigStore.GetConfigAsync();
            var tile = config.Tiles.FirstOrDefault(t => t.Name == name);

            return View("Add", tile);
        }

        [HttpPost("add/switch")]
        public async Task<IActionResult> Save(SwitchTile tile)
        {
            if (ModelState.IsValid)
            {
                return await SaveBaseTile(ConfigStore, tile);
            }

            await PopulateSelectLists();
            return View("Add", tile);
        }
    }
}
