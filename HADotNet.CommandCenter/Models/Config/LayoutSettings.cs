using System.ComponentModel.DataAnnotations;

namespace HADotNet.CommandCenter.Models.Config
{
    public class LayoutSettings
    {
        [Required]
        [Range(240, 4096, ErrorMessage = "Is your device's screen really that big/small?")]
        [Display(Name = "Device Width (px)")]
        public int DeviceWidthPx { get; set; }

        [Required]
        [Range(240, 4096, ErrorMessage = "Is your device's screen really that big/small?")]
        [Display(Name = "Device Height (px)")]
        public int DeviceHeightPx { get; set; }

        [Required]
        [Range(10, 1000, ErrorMessage = "Tile size is out of range (10..1000)")]
        [Display(Name = "Base Tile Size (px)")]
        public int BaseTileSizePx { get; set; }

        [Required]
        [Range(0, 50, ErrorMessage = "Tile spacing is out of range (0..50)")]
        [Display(Name = "Tile Spacing (px)")]
        public int TileSpacingPx { get; set; }
    }
}
