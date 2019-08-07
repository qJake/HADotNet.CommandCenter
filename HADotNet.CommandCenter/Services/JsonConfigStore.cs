using HADotNet.CommandCenter.Models;
using HADotNet.CommandCenter.Models.Config;
using HADotNet.CommandCenter.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Services
{
    public class JsonConfigStore : IConfigStore
    {
        private const string CONFIG_FILE = "config.json";
        private bool IsValid { get; set; }
        private string ConfigDirectory { get; set; }
        private string ConfigPath => Path.Combine(ConfigDirectory, CONFIG_FILE);
        public HaccOptions Options { get; }

        public JsonConfigStore(IOptions<HaccOptions> haccOptions)
        {
            Options = haccOptions.Value;
            ConfigDirectory = Environment.ExpandEnvironmentVariables(Options.ConfigLocation);
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

                return JsonConvert.DeserializeObject<ConfigRoot>(contents);
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
                await File.WriteAllTextAsync(ConfigPath, JsonConvert.SerializeObject(config, Formatting.Indented));
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
