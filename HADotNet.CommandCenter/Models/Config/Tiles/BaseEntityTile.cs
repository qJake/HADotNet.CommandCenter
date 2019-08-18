using HADotNet.Core.Clients;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Models.Config.Tiles
{
    public abstract class BaseEntityTile : BaseTile
    {
        [Display(Name = "Entity ID")]
        [Required(ErrorMessage = "Choose an entity to monitor.")]
        public string EntityId { get; set; }

        public virtual async Task OnClick(ServiceClient serviceClient)
        {
            await serviceClient.CallService("homeassistant.toggle", new { entity_id = EntityId });
        }
    }
}
