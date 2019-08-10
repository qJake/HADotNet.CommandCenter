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
        /// Gets the type of tile from the <see cref="TileTypeAttribute" />.
        /// </summary>
        public string Type => GetType().GetCustomAttribute<TileTypeAttribute>()?.Name ?? "Unknown";
    }
}
