
using HADotNet.CommandCenter.Models.Config.Tiles;
using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.Core.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Controllers
{
    [Route("admin/camera")]
    public class CameraTileController : BaseTileController
    {
        public IConfigStore ConfigStore { get; }
        public EntityClient EntityClient { get; }

        public CameraTileController(IConfigStore configStore, EntityClient entityClient)
        {
            ConfigStore = configStore;
            EntityClient = entityClient;
        }

        [Route("add/camera")]
        public async Task<IActionResult> Add()
        {
            ViewBag.Entities = (await EntityClient.GetEntities("camera")).OrderBy(e => e).Select(e => new SelectListItem(e, e));
            return View();
        }

        [Route("edit/camera")]
        public async Task<IActionResult> Edit([FromQuery] string name)
        {
            var config = await ConfigStore.GetConfigAsync();

            var tile = config.Tiles.FirstOrDefault(t => t.Name == name);

            ViewBag.Entities = (await EntityClient.GetEntities("camera")).OrderBy(e => e).Select(e => new SelectListItem(e, e));

            return View("Add", tile);
        }

        [HttpPost("add/camera")]
        public async Task<IActionResult> Save(CameraTile tile)
        {
            if (ModelState.IsValid)
            {
                return await SaveBaseTile(ConfigStore, tile);
            }

            return View("Add", tile);
        }
    }
}
