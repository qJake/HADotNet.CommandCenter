using HADotNet.CommandCenter.Models.Config;
using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.CommandCenter.Utils;
using HADotNet.Core.Clients;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Controllers
{
    public class AdminController : Controller
    {
        public IConfigStore ConfigStore { get; }
        public EntityClient EntityClient { get; }

        public AdminController(IConfigStore configStore, EntityClient entityClient)
        {
            ConfigStore = configStore;
            EntityClient = entityClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Layout()
        {
            var config = await ConfigStore.GetConfigAsync();

            ViewBag.PreviewWidth = config.LayoutSettings?.DeviceWidthPx / 2;
            ViewBag.PreviewHeight = config.LayoutSettings?.DeviceHeightPx / 2;
            ViewBag.PreviewSize = config.LayoutSettings?.BaseTileSizePx / 2;
            ViewBag.Padding = config.LayoutSettings?.TileSpacingPx;
            ViewBag.PreviewPadding = config.LayoutSettings?.TileSpacingPx / 2;

            return View(config.Tiles);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLayoutSettings(LayoutSettings settings)
        {
            if (ModelState.IsValid)
            {
                TempData.AddSuccess("Saved layout settings successfully!");
                await ConfigStore.ManipulateConfig(c => c.LayoutSettings = settings);
            }
            else
            {
                TempData.AddError("Unable to save layout settings.");
            }
            return RedirectToAction("Layout");
        }
        
        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var config = await ConfigStore.GetConfigAsync();

            return View(config.Settings);
        }

        [HttpPost]
        public async Task<IActionResult> Settings(SystemSettings newSettings)
        {
            if (ModelState.IsValid)
            {
                await ConfigStore.ManipulateConfig(c => c.Settings = newSettings);
                
                TempData.AddSuccess("Saved settings successfully!");

                return RedirectToAction("Settings");
            }

            return View(newSettings);
        }
    }
}