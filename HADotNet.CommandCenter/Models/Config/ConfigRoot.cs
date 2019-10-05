using HADotNet.CommandCenter.Models.Config.Pages;
using HADotNet.CommandCenter.Models.Config.Themes;
using HADotNet.CommandCenter.Models.Config.Tiles;
using HADotNet.CommandCenter.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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
        [Obsolete]
        public LayoutSettings LayoutSettings { get; set; }

        /// <summary>
        /// Gets or sets the list of tiles.
        /// </summary>
        [Obsolete]
        public List<BaseTile> Tiles { get; set; }

        /// <summary>
        /// Gets or sets the main tile layout.
        /// </summary>
        [Obsolete]
        public List<TileLayout> TileLayout { get; set; }

        /// <summary>
        /// Gets or sets the pages, which contain tiles and layout info.
        /// </summary>
        public List<Page> Pages { get; set; }

        /// <summary>
        /// Gets or sets the current theme for the dashboard.
        /// </summary>
        public Theme CurrentTheme { get; set; }

        /// <summary>
        /// Gets a page definition by name.
        /// </summary>
        /// <param name="pageName">The name of the page to get.</param>
        /// <returns>The instance of the page, or <see langword="null" /> if the page does not exist.</returns>
        [JsonIgnore]
        public Page this[string pageName] => Pages.Any() ? Pages.FirstOrDefault(p => p.Name == pageName) : null;
    }
}
