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
    public class LightTileController : BaseTileController
    {
        public IConfigStore ConfigStore { get; }
        public EntityClient EntityClient { get; }

        public LightTileController(IConfigStore configStore, EntityClient entityClient)
        {
            ConfigStore = configStore;
            EntityClient = entityClient;
        }

        [Route("add/light")]
        public async Task<IActionResult> Add()
        {
            ViewBag.Entities = (await EntityClient.GetEntities("light")).OrderBy(e => e).Select(e => new SelectListItem(e, e));
            return View();
        }

        [Route("edit/light")]
        public async Task<IActionResult> Edit([FromQuery] string name)
        {
            var config = await ConfigStore.GetConfigAsync();

            var tile = config.Tiles.FirstOrDefault(t => t.Name == name);

            ViewBag.Entities = (await EntityClient.GetEntities("light")).OrderBy(e => e).Select(e => new SelectListItem(e, e));

            return View("Add", tile);
        }

        [HttpPost("add/light")]
        public async Task<IActionResult> Save(LightTile tile)
        {
            if (ModelState.IsValid)
            {
                return await SaveBaseTile(ConfigStore, tile);
            }

            return View("Add", tile);
        }
    }
}
