using HADotNet.CommandCenter.Models;
using HADotNet.CommandCenter.Models.Config;
using HADotNet.CommandCenter.Models.Config.Pages;
using HADotNet.CommandCenter.Models.Config.Themes;
using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.CommandCenter.Utils;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Services
{
    public class JsonConfigStore : IConfigStore
    {
        private const string CONFIG_FILE = "config.json";

        private const string LINUX_DATA_LOCATION = "/app/data/";

        public static JsonSerializerSettings SerializerSettings { get; } = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            Converters =
            {
                new TileTypeJsonConverter()
            }
        };

        private bool IsValid { get; set; }
        private string ConfigDirectory { get; set; }
        private string ConfigPath => Path.Combine(ConfigDirectory, CONFIG_FILE);
        public HaccOptions Options { get; }

        public JsonConfigStore(IOptions<HaccOptions> haccOptions)
        {
            Options = haccOptions.Value;
            ConfigDirectory = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && Options.ConfigLocation == "." ? LINUX_DATA_LOCATION : Environment.ExpandEnvironmentVariables(Options.ConfigLocation);
        }

        public async Task ManipulateConfig(params Action<ConfigRoot>[] changes)
        {
            var config = await GetConfigAsync();

            foreach (var change in changes)
            {
                change(config);
            }

            // Ensure all collections are non-null
            config.CurrentTheme ??= new Theme();
            config.Pages ??= new List<Page>();

            await SaveConfigAsync(config);
        }

        public async Task<ConfigRoot> GetConfigAsync()
        {
            if (CheckPermissions())
            {
                if (!File.Exists(ConfigPath))
                {
                    return new ConfigRoot();
                }

                var contents = await File.ReadAllTextAsync(ConfigPath);
                if (string.IsNullOrWhiteSpace(contents))
                {
                    return new ConfigRoot();
                }

                var cfg = JsonConvert.DeserializeObject<ConfigRoot>(contents, SerializerSettings);

                return cfg;
            }
            else
            {
                throw new Exception("Config location is invalid. Check path and permissions.");
            }
        }

        public async Task SaveConfigAsync(ConfigRoot config)
        {
            if (CheckPermissions())
            {
                await File.WriteAllTextAsync(ConfigPath, JsonConvert.SerializeObject(config, SerializerSettings));
            }
            else
            {
                throw new Exception("Config location is invalid. Check path and permissions.");
            }
        }

        /// <summary>
        /// Checks to see if we can write to, and delete, a temporary file. Ensures the requested config directory is R/W.
        /// </summary>
        private bool CheckPermissions()
        {
            if (IsValid) return true;

            var di = new DirectoryInfo(ConfigDirectory);

            if (!di.Exists) di.Create();

            var tmpFile = Path.Combine(ConfigDirectory, ".tmp-write");

            try
            {
                var file = File.Create(tmpFile);
                file.Flush();
                file.Close();
                file.Dispose();

                File.Delete(tmpFile);
            }
            catch
            {
                return false;
            }

            return IsValid = Directory.Exists(ConfigDirectory);
        }
    }
}
