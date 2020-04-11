using HADotNet.CommandCenter.Models.Config;
using HADotNet.CommandCenter.Models.Config.Themes;
using HADotNet.CommandCenter.Services;
using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.CommandCenter.Utils;
using HADotNet.CommandCenter.ViewModels;
using HADotNet.Core;
using HADotNet.Core.Clients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Controllers
{
    public class AdminController : Controller
    {
        public IConfigStore ConfigStore { get; }
        public EntityClient EntityClient { get; }
        public ILogger<AdminController> Logger { get; }
        public DiscoveryClient DiscoveryClient { get; }

        public AdminController(IConfigStore configStore, EntityClient entityClient, ILogger<AdminController> logger, DiscoveryClient discoveryClient)
        {
            ConfigStore = configStore;
            EntityClient = entityClient;
            Logger = logger;
            DiscoveryClient = discoveryClient;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult ThankYou() => View();

        [HttpGet]
        public IActionResult PageMigration() => View("MigratedToPages");

        [HttpGet]
        public async Task<IActionResult> Technical()
        {
            ViewBag.Env = ((Hashtable)Environment.GetEnvironmentVariables()).Cast<DictionaryEntry>().ToDictionary(k => k.Key?.ToString(), v => v.Value?.ToString());

            var discovery = await DiscoveryClient.GetDiscoveryInfo();

            return View(discovery);
        }

        
        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var config = await ConfigStore.GetConfigAsync();

            try
            {
                if (TempData["check-settings"] is bool b && b)
                {
                    // This doesn't check if the access token is valid...
                    var inst = await DiscoveryClient.GetDiscoveryInfo();

                    // ... but this does.
                    var entities = await EntityClient.GetEntities();

                    ViewBag.Instance = $"Home Assistant instance: <b>{inst.LocationName} (Version {inst.Version}) [{ inst.BaseUrl}]</b>";
                }
            }
            catch (Exception ex)
            {
                await ConfigStore.ManipulateConfig(c => c.Settings.AccessToken = c.Settings.BaseUri = null);

                ClientFactory.Reset();

                config = await ConfigStore.GetConfigAsync();

                TempData.Remove(AlertManager.GRP_SUCCESS);
                Logger.LogError(ex, "Invalid system settings entered, or unable to reach Home Assistant with the specified information.");
                TempData.AddError("The settings entered are not valid. HACC is unable to reach your Home Assistant instance. Try your entries again, consult the <a target=\"_blank\" href=\"https://github.com/qJake/HADotNet.CommandCenter/wiki/Initial-System-Setup\">setup guide</a> for help, or check the logs (console) for more information.");

                // Reloads the request showing the error (TempData doesn't commit until a reload).
                return RedirectToAction("Settings");
            }

            return View(config.Settings);
        }

        [HttpPost]
        public async Task<IActionResult> Settings([FromForm] SystemSettings newSettings)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    newSettings ??= new SystemSettings();
                    newSettings.BaseUri = newSettings.BaseUri?.TrimEnd('/');
                    await ConfigStore.ManipulateConfig(c => c.Settings = newSettings);

                    ClientFactory.Reset();

                    TempData["check-settings"] = true;

                    return RedirectToAction("Settings");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Saving system settings failed.");
                TempData.AddError("Unable to save system settings. See log output (console) for more information.");
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
            try
            {
                if (ModelState.IsValid)
                {
                    var config = await ConfigStore.GetConfigAsync();

                    await ConfigStore.ManipulateConfig(c => c.CurrentTheme = newTheme);

                    TempData.AddSuccess("Saved theme settings successfully!");

                    return RedirectToAction("Themes");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Saving theme settings failed.");
                TempData.AddError("Unable to save theme changes. See log output (console) for more information.");
            }
            return View("Themes", newTheme);
        }

        [HttpPost]
        public async Task<IActionResult> ImportTheme([FromForm] IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    TempData.AddWarning("No file was uploaded. Please try again.");
                    return RedirectToAction("Themes");
                }

                string contents;
                using (var sr = new StreamReader(file.OpenReadStream()))
                {
                    contents = await sr.ReadToEndAsync();
                }

                try
                {
                    var newTheme = JsonConvert.DeserializeObject<Theme>(contents);

                    var config = await ConfigStore.GetConfigAsync();

                    await ConfigStore.ManipulateConfig(c => c.CurrentTheme = newTheme);

                    TempData.AddSuccess($"Successfully imported theme file '{file.FileName}' successfully! <a href=\"/\">Go check out your dashboard!</a>");
                }
                catch
                {
                    Logger.LogWarning("File selected was not able to be parsed into a theme. Check file contents and try again.");
                    TempData.AddError("Import file was not a theme file or could not otherwise be imported. Check that the file is not malformed and try again.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Importing theme settings failed.");
                TempData.AddError("Unable to import theme. See log output (console) for more information.");
            }
            return RedirectToAction("Themes");
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
            await Response.Body.WriteAsync(themeData, 0, themeData.Length);
            await Response.Body.FlushAsync();

            Logger.LogInformation("Exported theme settings.");

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ImportConfig([FromForm] IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    TempData.AddWarning("No file was uploaded. Please try again.");
                    return RedirectToAction("Index");
                }

                string contents;
                using (var sr = new StreamReader(file.OpenReadStream()))
                {
                    contents = await sr.ReadToEndAsync();
                }

                try
                {
                    var newConfig = JsonConvert.DeserializeObject<ConfigRoot>(contents, JsonConfigStore.SerializerSettings);
                    newConfig.Settings = null;

                    var config = await ConfigStore.GetConfigAsync();
                    var oldSettings = config.Settings;

                    newConfig.Settings = oldSettings;

                    await ConfigStore.SaveConfigAsync(newConfig);

                    // Ensures arrays are non-null even if no actions are performed.
                    await ConfigStore.ManipulateConfig();

                    TempData.AddSuccess($"Successfully imported system configuration file '{file.FileName}'!");
                }
                catch (Exception dex)
                {
                    Logger.LogError(dex, "File selected was not able to be parsed into a config object. Check file contents and try again.");
                    TempData.AddError("Import file was not a configuration file or could not otherwise be imported. Check that the file is not malformed and try again. See log output for more information.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Importing system config settings failed.");
                TempData.AddError("Unable to import system configuration. See log output (console) for more information.");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ExportConfig()
        {
            var config = await ConfigStore.GetConfigAsync();

            config.Settings = null;

            var configJson = JsonConvert.SerializeObject(config, JsonConfigStore.SerializerSettings);
            var configData = Encoding.UTF8.GetBytes(configJson);
            var filename = $"hacc-export-{DateTime.Now:yyyyMMdd-HHmmss}.config.json";

            Response.ContentType = "application/octet-stream";
            Response.Headers["Content-Disposition"] = $"attachment; filename={filename}";
            Response.Headers["Content-Transfer-Encoding"] = "binary";
            await Response.Body.WriteAsync(configData, 0, configData.Length);
            await Response.Body.FlushAsync();

            TempData.AddWarning("WARNING: Be careful when importing this file into another HACC instance. If the names of Home Assistant entities are different on the target platform, HACC may experience errors!");

            Logger.LogInformation($"Exported system configuration to downloaded file '{filename}'.");

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ResetConfig()
        {
            await ConfigStore.SaveConfigAsync(new ConfigRoot());

            // This will reset defaults and initialize collections, even if no actions are passed in.
            await ConfigStore.ManipulateConfig();

            TempData.AddSuccess("Successfully reset HACC configuration.");

            return RedirectToAction("Index");
        }
    }
}