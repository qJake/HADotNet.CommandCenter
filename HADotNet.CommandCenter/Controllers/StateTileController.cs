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
    public class StateTileController : BaseTileController
    {
        public IConfigStore ConfigStore { get; }
        public EntityClient EntityClient { get; }

        public StateTileController(IConfigStore configStore, EntityClient entityClient)
        {
            ConfigStore = configStore;
            EntityClient = entityClient;
        }

        [Route("add/state")]
        public async Task<IActionResult> Add()
        {
            ViewBag.Entities = (await EntityClient.GetEntities()).OrderBy(e => e).Select(e => new SelectListItem(e, e));
            return View();
        }

        [Route("edit/state")]
        public async Task<IActionResult> Edit([FromQuery] string name)
        {
            var config = await ConfigStore.GetConfigAsync();

            var tile = config.Tiles.FirstOrDefault(t => t.Name == name);

            ViewBag.Entities = (await EntityClient.GetEntities()).OrderBy(e => e).Select(e => new SelectListItem(e, e));

            return View("Add", tile);
        }

        [HttpPost("add/state")]
        public async Task<IActionResult> Save(StateTile tile)
        {
            if (ModelState.IsValid)
            {
                return await SaveBaseTile(ConfigStore, tile);
            }

            ViewBag.Entities = (await EntityClient.GetEntities()).OrderBy(e => e).Select(e => new SelectListItem(e, e));
            return View("Add", tile);
        }
    }
}
