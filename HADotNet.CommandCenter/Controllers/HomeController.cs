using System.Linq;
using System.Threading.Tasks;
using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.CommandCenter.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HADotNet.CommandCenter.Controllers
{
    public class HomeController : Controller
    {
        public IConfigStore ConfigStore { get; }

        public HomeController(IConfigStore configStore)
        {
            ConfigStore = configStore;
        }

        public async Task<IActionResult> Index()
        {
            var config = await ConfigStore.GetConfigAsync();

            return View(new TileDisplayViewModel
            {
                PageLayout = config.LayoutSettings,
                Tiles = from t in config.Tiles
                        join layout in config.TileLayout on t.Name equals layout.Name into tileGroup
                        from l in tileGroup.DefaultIfEmpty(null)
                        select new TileWithLayoutViewModel
                        {
                            Tile = t,
                            Layout = l
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
