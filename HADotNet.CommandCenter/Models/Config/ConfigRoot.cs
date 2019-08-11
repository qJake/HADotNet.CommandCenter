using HADotNet.CommandCenter.Models.Config.Tiles;
using HADotNet.CommandCenter.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HADotNet.CommandCenter.Models.Config
{
    /// <summary>
    /// Root configuration object for the entire command center.
    /// </summary>
    public class ConfigRoot
    {
        /// <summary>
        /// Gets or sets the global system configuration.
        /// </summary>
        public SystemSettings Settings { get; set; }

        /// <summary>
        /// Gets or sets the layout-related settings.
        /// </summary>
        public LayoutSettings LayoutSettings { get; set; }

        /// <summary>
        /// Gets or sets the list of tiles.
        /// </summary>
        [JsonConverter(typeof(TileTypeJsonConverter))]
        public List<BaseTile> Tiles { get; set; }

        /// <summary>
        /// Gets or sets the main tile layout.
        /// </summary>
        public List<TileLayout> TileLayout { get; set; }

        public ConfigRoot()
        {
            // Populate some sensible defaults, so lists aren't null for example
            Tiles = new List<BaseTile>();
        }
    }
}
