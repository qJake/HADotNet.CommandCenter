using HADotNet.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace HADotNet.CommandCenter.Models.Config.Tiles
{
    [TileType("state")]
    [TileIcon(TileIconType.Material, "numeric")]
    public class StateTile : BaseEntityTile
    {
        /// <summary>
        /// Gets or sets the refresh rate for this tile. A value of 0 indicates no refresh, unless the webpage itself refreshes.
        /// </summary>
        [Display(Name = "Refresh Rate")]
        [Range(0, 86400, ErrorMessage = "Enter a value between 0 and 86400.")]
        public int RefreshRate { get; set; }

        /// <summary>
        /// Gets or sets the override label for this tile.
        /// </summary>
        [Display(Name = "Override Label")]
        public string OverrideLabel { get; set; }

        /// <summary>
        /// Gets or sets if the state should be displayed as a whole number, if appropriate.
        /// </summary>
        [Display(Name = "Round Decimals")]
        public bool RoundDecimals { get; set; }

        /// <summary>
        /// Gets or sets a label that should be used when the entity is "on".
        /// </summary>
        [Display(Name = "Override 'On' Text")]
        public string DisplayTextOn { get; set; }

        /// <summary>
        /// Gets or sets a label that should be used when the entity is "off".
        /// </summary>
        [Display(Name = "Override 'Off' Text")]
        public string DisplayTextOff { get; set; }
    }
}
