using HADotNet.CommandCenter.Hubs;
using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.CommandCenter.Utils;
using HADotNet.Core.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Controllers
{
    [Route("admin/pages/{page}/tile")]
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

        public async Task<IActionResult> Index([FromRoute] string page)
        {
            var config = await ConfigStore.GetConfigAsync();

            return View(config[page].Tiles);
        }

        [Route("add")]
        public IActionResult AddTile([FromRoute] string page) => View();

        [Route("edit")]
        public async Task<IActionResult> EditTile([FromRoute] string page, [FromQuery] string name)
        {
            var config = await ConfigStore.GetConfigAsync();

            var tile = config[page].Tiles.FirstOrDefault(t => t.Name == name);

            return RedirectToAction("Edit", tile.TypeProper + "Tile", new { page, name });
        }

        [Route("delete")]
        public async Task<IActionResult> DeleteTile([FromRoute] string page, [FromQuery] string name)
        {
            var config = await ConfigStore.GetConfigAsync();

            var tile = config[page].Tiles.FirstOrDefault(t => t.Name == name);
            if (tile == null)
            {
                TempData.AddError($"Could not delete tile with name '{name}' (name not found).");
                return RedirectToAction("Index", new { page });
            }

            config[page].Tiles.Remove(tile);

            var layout = config[page].TileLayout.FirstOrDefault(l => l.Name == name);
            if (layout != null)
            {
                config[page].TileLayout.Remove(layout);
            }

            await ConfigStore.SaveConfigAsync(config);

            TempData.AddSuccess($"Tile '{name}' was deleted successfully.");

            return RedirectToAction("Index", new { page });
        }
    }
}