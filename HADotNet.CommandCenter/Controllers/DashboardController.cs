using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.CommandCenter.Utils;
using HADotNet.CommandCenter.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Controllers
{
    public class DashboardController : Controller
    {
        public IConfigStore ConfigStore { get; }

        public DashboardController(IConfigStore configStore)
        {
            ConfigStore = configStore;
        }

        [Route("d/{page?}")]
        public async Task<IActionResult> Index([FromRoute] string page)
        {
            var config = await ConfigStore.GetConfigAsync();

            if (string.IsNullOrWhiteSpace(page))
            {
                page = config.Pages.FirstOrDefault(p => p.IsDefaultPage)?.Name;
            }

            if (string.IsNullOrWhiteSpace(page))
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(config?.Settings?.AccessToken))
            {
                TempData.AddError("Unable to load dashboard - missing the access token. Visit Settings to fix.");
                return RedirectToAction("Admin", "Index");
            }

            if (string.IsNullOrWhiteSpace(config?.Settings?.BaseUri))
            {
                TempData.AddError("Unable to load dashboard - missing the system's Base URL. Visit Settings to fix.");
                return RedirectToAction("Admin", "Index");
            }

            return View(new TileDisplayViewModel
            {
                SystemSettings = config.Settings,
                CurrentPage = config[page],
                PageLayout = config[page].LayoutSettings,
                Theme = config.CurrentTheme,
                Tiles = config[page].Tiles == null
                        ? null
                        : from t in config[page].Tiles
                          join layout in config[page].TileLayout on t.Name equals layout.Name into tileGroup
                          from l in tileGroup.DefaultIfEmpty(null)
                          select new TileWithLayoutViewModel
                          {
                              Tile = t,
                              Layout = l,
                              Settings = config[page].LayoutSettings
                          }
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
