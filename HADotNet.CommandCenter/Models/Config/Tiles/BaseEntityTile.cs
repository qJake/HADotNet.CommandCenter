using System.ComponentModel.DataAnnotations;

namespace HADotNet.CommandCenter.Models.Config.Tiles
{
    public abstract class BaseEntityTile : BaseTile
    {
        [Display(Name = "Entity ID")]
        [Required(ErrorMessage = "Choose an entity to monitor.")]
        public string EntityId { get; set; }
    }
}
