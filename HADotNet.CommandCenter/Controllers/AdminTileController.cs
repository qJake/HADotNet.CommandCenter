using HADotNet.CommandCenter.Models.Config;
using HADotNet.CommandCenter.Models.Config.Tiles;
using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.CommandCenter.Utils;
using HADotNet.Core.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Controllers
{
    [Route("admin/tile")]
    public class AdminTileController : Controller
    {
        public IConfigStore ConfigStore { get; }
        public EntityClient EntityClient { get; }

        public AdminTileController(IConfigStore configStore, EntityClient entityClient)
        {
            ConfigStore = configStore;
            EntityClient = entityClient;
        }

        public async Task<IActionResult> Index()
        {
            var config = await ConfigStore.GetConfigAsync();

            return View(config.Tiles);
        }

        [Route("add")]
        public IActionResult AddTile()
        {
            return View();
        }

        [Route("add/state")]
        public async Task<IActionResult> AddTileState()
        {
            ViewBag.Entities = (await EntityClient.GetEntities()).OrderBy(e => e).Select(e => new SelectListItem(e, e));
            return View();
        }

        [Route("delete")]
        public async Task<IActionResult> DeleteTile([FromQuery] string name)
        {
            var config = await ConfigStore.GetConfigAsync();

            var tile = config.Tiles.FirstOrDefault(t => t.Name == name);
            if (tile == null)
            {
                TempData.AddError($"Could not delete tile with name '{name}' (name not found).");
                return RedirectToAction("Index");
            }

            config.Tiles.Remove(tile);

            await ConfigStore.SaveConfigAsync(config);

            TempData.AddSuccess($"Tile '{name}' was deleted successfully.");

            return RedirectToAction("Index");
        }

        [HttpPost("add/state")]
        public async Task<IActionResult> SaveTileState(StateTile tile)
        {
            if (ModelState.IsValid)
            {
                await ConfigStore.ManipulateConfig(c => c.Tiles.Add(tile));

                return RedirectToAction("Index");
            }

            return View("AddTileState", tile);
        }
    }
}