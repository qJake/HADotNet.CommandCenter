using System;
using System.ComponentModel.DataAnnotations;

namespace HADotNet.CommandCenter.Models.Config.Tiles
{
    [TileType("camera")]
    [TileIcon(TileIconType.Material, "camera")]
    public class CameraTile : BaseEntityTile
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
    }
}
