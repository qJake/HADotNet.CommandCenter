using HADotNet.CommandCenter.Models.Config;
using HADotNet.CommandCenter.Models.Config.Pages;
using HADotNet.CommandCenter.Models.Config.Tiles;
using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.CommandCenter.Utils;
using HADotNet.CommandCenter.ViewModels;
using HADotNet.Core.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Controllers
{
    [Route("admin/pages")]
    public class AdminPagesController : Controller
    {
        public IConfigStore ConfigStore { get; }
        public EntityClient EntityClient { get; }
        public ILogger<AdminController> Logger { get; }
        public DiscoveryClient DiscoveryClient { get; }

        public AdminPagesController(IConfigStore configStore, EntityClient entityClient, ILogger<AdminController> logger, DiscoveryClient discoveryClient)
        {
            ConfigStore = configStore;
            EntityClient = entityClient;
            Logger = logger;
            DiscoveryClient = discoveryClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var config = await ConfigStore.GetConfigAsync();

            return View(config.Pages);
        }

        [HttpGet("add")]
        public IActionResult AddPage() => View();

        [HttpPost("add")]
        public async Task<IActionResult> AddPage([FromForm] Page newPage)
        {
            if (ModelState.IsValid)
            {
                var defaultLayout = (await ConfigStore.GetConfigAsync()).Pages.FirstOrDefault(p => p.IsDefaultPage)?.LayoutSettings;
                if (newPage.IsDefaultPage)
                {
                    await ConfigStore.ManipulateConfig(c => c.Pages.ForEach(p => p.IsDefaultPage = false));
                }

                newPage.TileLayout = new List<TileLayout>();
                newPage.Tiles = new List<BaseTile>();
                newPage.LayoutSettings = new LayoutSettings
                {
                    BaseTileSizePx = defaultLayout?.BaseTileSizePx ?? 100,
                    TileSpacingPx = defaultLayout?.TileSpacingPx ?? 6,
                    DeviceHeightPx = defaultLayout?.DeviceHeightPx ?? 860,
                    DeviceWidthPx = defaultLayout?.DeviceWidthPx ?? 965
                };

                await ConfigStore.ManipulateConfig(c => c.Pages.Add(newPage));

                TempData.AddSuccess($"Successfully added page '{newPage.Description}'!");
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet("{page}/delete")]
        public async Task<IActionResult> DeletePageConfirmation([FromRoute] string page)
        {
            var cfg = await ConfigStore.GetConfigAsync();
            var p = cfg[page];

            if (cfg.Pages.Count == 1)
            {
                TempData.AddWarning("You must have at least one page. Please create a new page before attempting to delete this one.");
                return RedirectToAction("Index");
            }

            if (p == null)
            {
                TempData.AddWarning($"The specified page '{page}' does not exist in the configuration.");
                return RedirectToAction("Index");
            }

            if (p.IsDefaultPage)
            {
                TempData.AddWarning($"You can't delete the default page. Assign another page as the default first, then delete this one.");
                return RedirectToAction("Index");
            }

            return View(p);
        }
        
        [HttpPost("{page}/delete")]
        public async Task<IActionResult> DeletePage([FromRoute] string page)
        {
            var cfg = await ConfigStore.GetConfigAsync();
            var p = cfg[page];

            if (cfg.Pages.Count == 1 || (p?.IsDefaultPage ?? false))
            {
                TempData.AddWarning("You have to have at least one page, and you can't delete the default page.");
                return RedirectToAction("Index");
            }

            if (p == null)
            {
                TempData.AddWarning($"The specified page '{page}' does not exist in the configuration.");
                return RedirectToAction("Index");
            }

            await ConfigStore.ManipulateConfig(c => c.Pages.Remove(c[page]));
            TempData.AddSuccess($"The page '{page}' was successfully deleted.");

            return RedirectToAction("Index");
        }

        [HttpGet("{page}/edit")]
        public async Task<IActionResult> EditPage([FromRoute] string page)
        {
            var cfg = await ConfigStore.GetConfigAsync();
            var p = cfg[page];

            if (p == null)
            {
                TempData.AddWarning($"The specified page '{page}' does not exist in the configuration.");
                return RedirectToAction("Index");
            }

            return View(p);
        }

        [HttpPost("{page}/edit")]
        public async Task<IActionResult> EditPage([FromForm] Page page)
        {
            if (ModelState.IsValid)
            {
                if (page.IsDefaultPage)
                {
                    await ConfigStore.ManipulateConfig(c => c.Pages.ForEach(p => p.IsDefaultPage = false));
                }

                await ConfigStore.ManipulateConfig(c =>
                {
                    c[page.Name].Description = page.Description;
                    c[page.Name].IsDefaultPage = page.IsDefaultPage;
                    c[page.Name].AutoReturnSeconds = page.AutoReturnSeconds;
                });

                TempData.AddSuccess($"Successfully edited page '{page.Description}'!");
                return RedirectToAction("Index");
            }
            return View(page);
        }

        [HttpGet("{page}/layout")]
        public async Task<IActionResult> Layout([FromRoute] string page)
        {
            var config = await ConfigStore.GetConfigAsync();

            ViewBag.PreviewWidth = config[page].LayoutSettings?.DeviceWidthPx / 2.0;
            ViewBag.PreviewHeight = config[page].LayoutSettings?.DeviceHeightPx / 2.0;
            ViewBag.PreviewSize = config[page].LayoutSettings?.BaseTileSizePx / 2.0;
            ViewBag.Padding = config[page].LayoutSettings?.TileSpacingPx;
            ViewBag.PreviewPadding = config[page].LayoutSettings?.TileSpacingPx / 2.0;

            return View(from t in config[page].Tiles ?? new List<BaseTile>()
                        join layout in config[page].TileLayout on t.Name equals layout.Name into tileGroup
                        from l in tileGroup.DefaultIfEmpty(null)
                        select new TileWithLayoutViewModel
                        {
                            Tile = t,
                            Layout = l,
                            Settings = config[page].LayoutSettings
                        });
        }

        [HttpPost("{page}/layoutSettings")]
        public async Task<IActionResult> UpdateLayoutSettings([FromRoute] string page, LayoutSettings settings)
        {
            if (ModelState.IsValid)
            {
                TempData.AddSuccess("Saved layout settings successfully!");
                await ConfigStore.ManipulateConfig(c => c[page].LayoutSettings = settings);
            }
            else
            {
                TempData.AddError("Unable to save layout settings.");
            }
            return RedirectToAction("Layout", new { page });
        }

        [HttpPost("{page}/layout")]
        public async Task<IActionResult> UpdateLayout([FromRoute] string page)
        {
            var val = Request.Form["tilelayout"][0];

            try
            {
                var layout = JsonConvert.DeserializeObject<List<TileLayout>>(val);

                // Multiply the X and Y by 2 since the preview is at 50%.
                layout.ForEach(l => { l.XPos *= 2; l.YPos *= 2; });

                await ConfigStore.ManipulateConfig(c => c[page].TileLayout = layout);

                TempData.AddSuccess("Tile layout saved successfully!");
            }
            catch
            {
                TempData.AddError("Unable to read tile layout from page. Cannot write to config file.");
            }

            return RedirectToAction("Layout", new { page });
        }
    }
}