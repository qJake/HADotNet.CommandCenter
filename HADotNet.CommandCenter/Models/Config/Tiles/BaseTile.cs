using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace HADotNet.CommandCenter.Models.Config.Tiles
{
    public class BaseTile
    {
        /// <summary>
        /// Gets or sets the tile's admin name.
        /// </summary>
        [Display(Name = "Tile Name")]
        [Required(ErrorMessage = "Give it a name!")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Only a-z, 0-9, dashes and underscores.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the refresh rate for this tile. A value of 0 indicates no refresh, unless the webpage itself refreshes.
        /// </summary>
        [Display(Name = "Refresh Rate")]
        [Range(0, 86400, ErrorMessage = "Enter a value between 0 and 86400.")]
        public int RefreshRate { get; set; }

        /// <summary>
        /// Gets the type of tile from the <see cref="TileTypeAttribute" />.
        /// </summary>
        public string Type => GetType().GetCustomAttribute<TileTypeAttribute>()?.Name ?? "Unknown";
    }
}
