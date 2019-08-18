using System.ComponentModel.DataAnnotations;

namespace HADotNet.CommandCenter.Models.Config.Tiles
{
    [TileType("switch")]
    [TileIcon(TileIconType.Material, "light-switch")]
    public class SwitchTile : BaseEntityTile
    {
        /// <summary>
        /// Gets or sets the override label for this tile.
        /// </summary>
        [Display(Name = "Override Label")]
        public string OverrideLabel { get; set; }

        /// <summary>
        /// Gets or sets the display icon for this light.
        /// </summary>
        [Display(Name = "Display Icon")]
        public string DisplayIcon { get; set; }

        /// <summary>
        /// Gets or sets the display icon for this light when the state is off.
        /// </summary>
        [Display(Name = "Display Off Icon")]
        public string DisplayOffIcon { get; set; }

        /// <summary>
        /// Gets or sets the CSS color when the light is on.
        /// </summary>
        [Display(Name = "On Color")]
        public string OnColor { get; set; }

        /// <summary>
        /// Gets or sets the CSS color when the light is off.
        /// </summary>
        [Display(Name = "Off Color")]
        public string OffColor { get; set; }
    }
}
