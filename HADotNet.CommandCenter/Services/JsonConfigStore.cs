using HADotNet.CommandCenter.Models;
using HADotNet.CommandCenter.Models.Config;
using HADotNet.CommandCenter.Models.Config.Tiles;
using HADotNet.CommandCenter.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Services
{
    public class JsonConfigStore : IConfigStore
    {
        private const string CONFIG_FILE = "config.json";

        private static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
    };

        private bool IsValid { get; set; }
        private string ConfigDirectory { get; set; }
        private string ConfigPath => Path.Combine(ConfigDirectory, CONFIG_FILE);
        public HaccOptions Options { get; }

        public JsonConfigStore(IOptions<HaccOptions> haccOptions)
        {
            Options = haccOptions.Value;
            ConfigDirectory = Environment.ExpandEnvironmentVariables(Options.ConfigLocation);
        }

        public async Task ManipulateConfig(params Action<ConfigRoot>[] changes)
        {
            var config = await GetConfigAsync();

            foreach (var change in changes)
            {
                change(config);
            }

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

                // Perform some cleanup - if lists are null, for example
                cfg.Tiles = cfg.Tiles ?? new List<BaseTile>();

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
