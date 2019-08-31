using HADotNet.CommandCenter.Models.Config;
using HADotNet.CommandCenter.Models.Config.Themes;
using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.CommandCenter.Utils;
using HADotNet.CommandCenter.ViewModels;
using HADotNet.Core.Clients;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            ViewBag.PreviewWidth = config.LayoutSettings?.DeviceWidthPx / 2.0;
            ViewBag.PreviewHeight = config.LayoutSettings?.DeviceHeightPx / 2.0;
            ViewBag.PreviewSize = config.LayoutSettings?.BaseTileSizePx / 2.0;
            ViewBag.Padding = config.LayoutSettings?.TileSpacingPx;
            ViewBag.PreviewPadding = config.LayoutSettings?.TileSpacingPx / 2.0;

            return View(from t in config.Tiles
                        join layout in config?.TileLayout on t.Name equals layout.Name into tileGroup
                        from l in tileGroup.DefaultIfEmpty(null)
                        select new TileWithLayoutViewModel
                        {
                            Tile = t,
                            Layout = l,
                            Settings = config.LayoutSettings
                        });
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
        
        [HttpPost]
        public async Task<IActionResult> UpdateLayout()
        {
            var val = Request.Form["tilelayout"][0];

            try
            {
                var layout = JsonConvert.DeserializeObject<List<TileLayout>>(val);

                // Multiply the X and Y by 2 since the preview is at 50%.
                layout.ForEach(l => { l.XPos *= 2; l.YPos *= 2; });

                await ConfigStore.ManipulateConfig(c => c.TileLayout = layout);

                TempData.AddSuccess("Tile layout saved successfully!");
            }
            catch
            {
                TempData.AddError("Unable to read tile layout from page. Cannot write to config file.");
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
                newSettings.BaseUri = newSettings.BaseUri.TrimEnd('/');
                await ConfigStore.ManipulateConfig(c => c.Settings = newSettings);
                
                TempData.AddSuccess("Saved settings successfully!");

                return RedirectToAction("Settings");
            }

            return View(newSettings);
        }

        [HttpGet]
        public async Task<IActionResult> Themes()
        {
            var config = await ConfigStore.GetConfigAsync();

            return View(config.CurrentTheme);
        }

        [HttpPost]
        public async Task<IActionResult> SaveTheme([FromForm] Theme newTheme)
        {
            if (ModelState.IsValid)
            {
                var config = await ConfigStore.GetConfigAsync();

                await ConfigStore.ManipulateConfig(c => c.CurrentTheme = newTheme);

                TempData.AddSuccess("Saved theme settings successfully!");

                return RedirectToAction("Themes");
            }

            return View("Themes", newTheme);
        }

        [HttpGet]
        public async Task<IActionResult> ExportTheme()
        {
            var config = await ConfigStore.GetConfigAsync();
            var theme = JsonConvert.SerializeObject(config.CurrentTheme, Formatting.Indented);
            var themeData = Encoding.UTF8.GetBytes(theme);

            Response.ContentType = "application/octet-stream";
            Response.Headers["Content-Disposition"] = "attachment; filename=hacc-export.theme.json";
            Response.Headers["Content-Transfer-Encoding"] = "binary";
            Response.Body.Write(themeData, 0, themeData.Length);
            Response.Body.Flush();

            return Ok();
        }
    }
}