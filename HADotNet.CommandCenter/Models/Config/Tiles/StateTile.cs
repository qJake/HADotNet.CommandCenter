using System.ComponentModel.DataAnnotations;

namespace HADotNet.CommandCenter.Models.Config.Tiles
{
    [TileType("state")]
    public class StateTile : BaseTile
    {
        [Display(Name = "Entity ID")]
        [Required(ErrorMessage = "Choose an entity to monitor.")]
        public string EntityId { get; set; }
    }
}
