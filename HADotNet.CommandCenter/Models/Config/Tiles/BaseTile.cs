using HADotNet.Core.Models;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace HADotNet.CommandCenter.Models.Config.Tiles
{
    public abstract class BaseTile
    {
        /// <summary>
        /// Gets or sets the tile's admin name.
        /// </summary>
        [Display(Name = "Tile Name")]
        [Required(ErrorMessage = "Give it a name!")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Only a-z, 0-9, dashes and underscores.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the horizontal size multiplier.
        /// </summary>
        [Display(Name = "Tile Width")]
        [Range(1, 4)]
        public int SizeX { get; set; }

        /// <summary>
        /// Gets or sets the vertical size multiplier.
        /// </summary>
        [Display(Name = "Tile Height")]
        [Range(1, 4)]
        public int SizeY { get; set; }

        /// <summary>
        /// Gets the type of tile from the <see cref="TileTypeAttribute" />.
        /// </summary>
        public string Type => GetType().GetCustomAttribute<TileTypeAttribute>()?.Name ?? "Unknown";

        /// <summary>
        /// Gets the type of tile from the <see cref="TileTypeAttribute" />, expressed in proper case (capitalized).
        /// </summary>
        [JsonIgnore]
        public string TypeProper => Type[0].ToString().ToUpper() + new string(Type.Skip(1).ToArray());

        /// <summary>
        /// Gets the full icon class name for this tile.
        /// </summary>
        [JsonIgnore]
        public string IconClassName
        {
            get
            {
                var iconInfo = GetType().GetCustomAttribute<TileIconAttribute>();
                if (iconInfo == null)
                {
                    return "";
                }
                switch (iconInfo.IconType)
                {
                    case TileIconType.Material: return $"mdi mdi-24px mdi-{iconInfo.IconName}";
                    case TileIconType.Semantic: return $"{iconInfo.IconName} icon";
                    default: return iconInfo.IconName;
                }
            }
        }

        /// <summary>
        /// Allows each tile to manipulate a state object before it is sent to the UI - for example, rounding a decimal value.
        /// </summary>
        /// <param name="state">The original state object.</param>
        public virtual StateObject StateManipulator(StateObject state) => state;

        public int GetTileSizeX(int tileSize, int padding) => Math.Min(Math.Max(SizeX, 1), 4) * tileSize + (Math.Min(Math.Max(SizeX, 1), 4) - 1) * padding;
        public int GetTileSizeY(int tileSize, int padding) => Math.Min(Math.Max(SizeY, 1), 4) * tileSize + (Math.Min(Math.Max(SizeY, 1), 4) - 1) * padding;
    }
}
