using System.ComponentModel.DataAnnotations;

namespace HADotNet.CommandCenter.Models.Config.Tiles
{
    [TileType("label")]
    [TileIcon(TileIconType.Material, "format-textbox")]
    public class LabelTile : BaseTile
    {
        /// <summary>
        /// Gets or sets the text in the label.
        /// </summary>
        [Required]
        [Display(Name = "Label Text")]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the CSS class(es) to apply to the label.
        /// </summary>
        [Display(Name = "CSS Class(es)")]
        public string CssClass { get; set; }
    }
}
