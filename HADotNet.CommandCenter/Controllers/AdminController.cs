using HADotNet.CommandCenter.Models.Config;
using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.CommandCenter.Utils;
using HADotNet.Core.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
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

        public IActionResult Tiles()
        {
            return View();
        }

        public async Task<IActionResult> AddTileState()
        {
            ViewBag.Entities = (await EntityClient.GetEntities()).OrderBy(e => e).Select(e => new SelectListItem(e, e));
            return View();
        }

        public IActionResult Layout()
        {
            return View();
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
                var config = await ConfigStore.GetConfigAsync();
                config.Settings = newSettings;
                await ConfigStore.SaveConfigAsync(config);

                TempData.AddSuccess("Saved settings successfully!");

                return RedirectToAction("Settings");
            }

            return View(newSettings);
        }
    }
}