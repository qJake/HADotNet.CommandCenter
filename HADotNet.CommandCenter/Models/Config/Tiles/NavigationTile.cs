using System.ComponentModel.DataAnnotations;

namespace HADotNet.CommandCenter.Models.Config.Tiles
{
    [TileType("navigation")]
    [TileIcon(TileIconType.Material, "arrow-decision")]
    public class NavigationTile : BaseTile
    {
        /// <summary>
        /// Gets or sets the override label for this tile.
        /// </summary>
        [Required]
        [Display(Name = "Navigation Mode")]
        public string Mode { get; set; }

        /// <summary>
        /// Gets or sets if the state should be displayed as a whole number, if appropriate.
        /// </summary>
        [Display(Name = "Target Page")]
        public string Target { get; set; }

        /// <summary>
        /// Gets or sets the override label for this tile.
        /// </summary>
        [Display(Name = "Label")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the display icon for this light.
        /// </summary>
        [Display(Name = "Display Icon")]
        public string DisplayIcon { get; set; }
    }
}
