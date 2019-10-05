using HADotNet.CommandCenter.Models.Config.Tiles;
using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.Core.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Controllers
{
    [Route("admin/pages/{page}/tile")]
    public class NavigationTileController : BaseTileController
    {
        public IConfigStore ConfigStore { get; }
        public EntityClient EntityClient { get; }

        public NavigationTileController(IConfigStore configStore, EntityClient entityClient)
        {
            ConfigStore = configStore;
            EntityClient = entityClient;
        }

        [Route("add/navigation")]
        public async Task<IActionResult> Add()
        {
            var config = await ConfigStore.GetConfigAsync();
            ViewBag.Pages = config.Pages.Select(p => new SelectListItem($"{p.Description} ({p.Name})", p.Name));

            return View();
        }

        [Route("edit/navigation")]
        public async Task<IActionResult> Edit([FromRoute] string page, string name)
        {
            var config = await ConfigStore.GetConfigAsync();
            ViewBag.Pages = config.Pages.Select(p => new SelectListItem($"{p.Description} ({p.Name})", p.Name));

            var tile = config[page].Tiles.FirstOrDefault(t => t.Name == name);

            return View("Add", tile);
        }

        [HttpPost("add/navigation")]
        public async Task<IActionResult> Save([FromRoute] string page, NavigationTile tile)
        {
            if (ModelState.IsValid)
            {
                return await SaveBaseTile(page, ConfigStore, tile);
            }

            return View("Add", tile);
        }
    }
}
