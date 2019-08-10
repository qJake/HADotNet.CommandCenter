using HADotNet.CommandCenter.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.ViewComponents
{
    public class LayoutSettingsViewComponent : ViewComponent
    {
        public IConfigStore ConfigStore { get; }

        public LayoutSettingsViewComponent(IConfigStore configStore)
        {
            ConfigStore = configStore;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var config = await ConfigStore.GetConfigAsync();

            return View(config.LayoutSettings);
        }
    }
}
