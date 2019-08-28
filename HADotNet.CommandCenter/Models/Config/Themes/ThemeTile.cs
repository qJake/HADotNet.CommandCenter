using System.ComponentModel.DataAnnotations;

namespace HADotNet.CommandCenter.Models.Config.Themes
{
    public class ThemeTile
    {
        [Display(Name = "Tile Background Color")]
        public string TileBackgroundColor { get; set; }

        [Display(Name = "Tile Border Radius (px)")]
        public int TileBorderRadius { get; set; }
    }
}
