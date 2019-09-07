using System.ComponentModel.DataAnnotations;

namespace HADotNet.CommandCenter.Models.Config.Themes
{
    public class ThemeTile
    {
        [Display(Name = "Tile Background Color")]
        public string TileBackgroundColor { get; set; }

        [Display(Name = "Tile Font Color")]
        public string TileFontColor { get; set; }

        [Display(Name = "Tile Border Radius (px)")]
        public int? TileBorderRadius { get; set; } 

        [Display(Name = "Tile Box Shadow")]
        public string TileBoxShadow { get; set; }

        [Display(Name = "Default Icon On Color")]
        public string TileDefaultOnColor { get; set; }

        [Display(Name = "Default Icon Off Color")]
        public string TileDefaultOffColor { get; set; }
    }
}
