using HADotNet.CommandCenter.Hubs;
using HADotNet.CommandCenter.Models.Config.Tiles;
using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.CommandCenter.Utils;
using HADotNet.Core.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Controllers
{
    [Route("admin/tile")]
    public class AdminTileController : Controller
    {
        public IConfigStore ConfigStore { get; }
        public EntityClient EntityClient { get; }
        public IHubContext<TileHub, ITileHub> TileHub { get; }

        public AdminTileController(IConfigStore configStore, EntityClient entityClient, IHubContext<TileHub, ITileHub> hubContext)
        {
            ConfigStore = configStore;
            EntityClient = entityClient;
            TileHub = hubContext;
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

        [Route("add/blank")]
        public IActionResult AddTileBlank()
        {
            return View();
        }

        [Route("add/state")]
        public async Task<IActionResult> AddTileState()
        {
            ViewBag.Entities = (await EntityClient.GetEntities()).OrderBy(e => e).Select(e => new SelectListItem(e, e));
            return View();
        }

        [Route("add/light")]
        public async Task<IActionResult> AddTileLight()
        {
            ViewBag.Entities = (await EntityClient.GetEntities()).OrderBy(e => e).Select(e => new SelectListItem(e, e));
            return View();
        }

        [Route("edit")]
        public async Task<IActionResult> EditTile([FromQuery] string name)
        {
            ViewBag.Entities = (await EntityClient.GetEntities()).OrderBy(e => e).Select(e => new SelectListItem(e, e));

            var config = await ConfigStore.GetConfigAsync();

            var tile = config.Tiles.FirstOrDefault(t => t.Name == name);
            var typeName = tile.Type.ToUpper()[0] + new string(tile.Type.Skip(1).ToArray());
            return View("AddTile" + typeName, tile);
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

            var layout = config.TileLayout.FirstOrDefault(l => l.Name == name);
            if (layout != null)
            {
                config.TileLayout.Remove(layout);
            }

            await ConfigStore.SaveConfigAsync(config);

            TempData.AddSuccess($"Tile '{name}' was deleted successfully.");

            return RedirectToAction("Index");
        }

        [HttpPost("add/blank")]
        public async Task<IActionResult> SaveTileBlank(BlankTile tile)
        {
            if (ModelState.IsValid)
            {
                return await SaveBaseTile(tile);
            }

            return View("AddTileBlank", tile);
        }

        [HttpPost("add/state")]
        public async Task<IActionResult> SaveTileState(StateTile tile)
        {
            if (ModelState.IsValid)
            {
                return await SaveBaseTile(tile);
            }

            return View("AddTileState", tile);
        }

        [HttpPost("add/light")]
        public async Task<IActionResult> SaveTileLight(LightTile tile)
        {
            if (ModelState.IsValid)
            {
                return await SaveBaseTile(tile);
            }

            return View("AddTileLight", tile);
        }

        private async Task<IActionResult> SaveBaseTile(BaseTile tile)
        {
            await ConfigStore.ManipulateConfig(c =>
            {
                if (!string.IsNullOrWhiteSpace(Request.Form["originalName"]))
                {
                    var existing = c.Tiles.FirstOrDefault(t => t.Name == Request.Form["originalName"]);
                    if (existing == null)
                    {
                        TempData.AddError($"Unable to update tile with original name '{Request.Form["originalName"]}'.");
                    }

                    var i = c.Tiles.IndexOf(existing);
                    c.Tiles.RemoveAt(i);
                    c.Tiles.Insert(i, tile);
                }
                else
                {
                    c.Tiles.Add(tile);
                }
            });

            TempData.AddSuccess($"Successfully saved {tile.Type} tile '{tile.Name}'.");

            return RedirectToAction("Index");
        }
    }
}