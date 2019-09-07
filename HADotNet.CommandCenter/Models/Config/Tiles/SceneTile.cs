using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using HADotNet.Core.Clients;

namespace HADotNet.CommandCenter.Models.Config.Tiles
{
    [TileType("scene")]
    [TileIcon(TileIconType.Material, "library-movie")]
    public class SceneTile : BaseEntityTile
    {
        /// <summary>
        /// Gets or sets the override label for this tile.
        /// </summary>
        [Display(Name = "Override Label")]
        public string OverrideLabel { get; set; }

        /// <summary>
        /// Gets or sets the display icon for this light.
        /// </summary>
        [Display(Name = "Display Icon")]
        public string DisplayIcon { get; set; }

        /// <summary>
        /// Gets or sets the CSS color when the light is off.
        /// </summary>
        [Display(Name = "Icon Color")]
        public string IconColor { get; set; }

        public override async Task OnClick(ServiceClient serviceClient)
        {
            await serviceClient.CallService("scene.turn_on", new { entity_id = EntityId });
        }
    }
}
