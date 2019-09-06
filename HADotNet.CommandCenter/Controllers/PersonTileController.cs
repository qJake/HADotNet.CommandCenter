using HADotNet.CommandCenter.Models.Config.Tiles;
using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.Core.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Controllers
{
    [Route("admin/tile")]
    public class PersonTileController : BaseTileController
    {
        public IConfigStore ConfigStore { get; }
        public EntityClient EntityClient { get; }

        public PersonTileController(IConfigStore configStore, EntityClient entityClient)
        {
            ConfigStore = configStore;
            EntityClient = entityClient;
        }
        private async Task PopulateSelectLists()
        {
            ViewBag.Entities = (await EntityClient.GetEntities("device_tracker"))
                .Union(await EntityClient.GetEntities("person"))
                .OrderBy(e => e)
                .Select(e => new SelectListItem(e, e));
        }

        [Route("add/person")]
        public async Task<IActionResult> Add()
        {
            await PopulateSelectLists();
            return View();
        }

        [Route("edit/person")]
        public async Task<IActionResult> Edit([FromQuery] string name)
        {
            var config = await ConfigStore.GetConfigAsync();

            var tile = config.Tiles.FirstOrDefault(t => t.Name == name);

            await PopulateSelectLists();

            return View("Add", tile);
        }

        [HttpPost("add/person")]
        public async Task<IActionResult> Save(PersonTile tile)
        {
            if (ModelState.IsValid)
            {
                return await SaveBaseTile(ConfigStore, tile);
            }

            await PopulateSelectLists();
            return View("Add", tile);
        }
    }
}
