using System.ComponentModel.DataAnnotations;

namespace HADotNet.CommandCenter.Models.Config.Tiles
{
    [TileType("person")]
    [TileIcon(TileIconType.Material, "account-circle")]
    public class PersonTile : BaseEntityTile
    {
        /// <summary>
        /// Gets or sets the override label for this tile.
        /// </summary>
        [Display(Name = "Override Label")]
        public string OverrideLabel { get; set; }
    }
}
