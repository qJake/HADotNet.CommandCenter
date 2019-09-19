using System;
using System.ComponentModel.DataAnnotations;
using HADotNet.Core.Models;

namespace HADotNet.CommandCenter.Models.Config.Tiles
{
    [TileType("media")]
    [TileIcon(TileIconType.Material, "television-classic")]
    public class MediaTile : BaseEntityTile
    {
        /// <summary>
        /// Gets or sets the refresh rate for this tile, in seconds.
        /// </summary>
        [Display(Name = "Refresh Rate")]
        [Range(1, 600, ErrorMessage = "Enter a value between 1 and 600.")]
        public int RefreshRate { get; set; }

        /// <summary>
        /// Gets or sets the image crop mode for displaying the image.
        /// </summary>
        [Display(Name = "Image Crop Mode")]
        public string ImageCropMode { get; set; }

        /// <summary>
        /// Gets or sets if we should show what's playing.
        /// </summary>
        [Display(Name = "Show Media Title")]
        public bool ShowTitle { get; set; }

        /// <summary>
        /// Gets or sets if we should show the name of the entity.
        /// </summary>
        [Display(Name = "Show Entity Label")]
        public bool ShowLabel { get; set; }

        /// <summary>
        /// Gets or sets the override label for this tile.
        /// </summary>
        [Display(Name = "Override Label")]
        public string OverrideLabel { get; set; }

    }
}
