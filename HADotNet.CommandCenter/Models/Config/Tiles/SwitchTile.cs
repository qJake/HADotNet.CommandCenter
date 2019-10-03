using HADotNet.Core;
using HADotNet.Core.Clients;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Models.Config.Tiles
{
    [TileType("switch")]
    [TileIcon(TileIconType.Material, "light-switch")]
    public class SwitchTile : BaseEntityTile
    {
        /// <summary>
        /// Gets or sets the refresh rate for this tile. A value of 0 indicates no refresh, unless the webpage itself refreshes.
        /// </summary>
        [Display(Name = "Refresh Rate")]
        [Range(0, 86400, ErrorMessage = "Enter a value between 0 and 86400.")]
        public int RefreshRate { get; set; }

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
        /// Gets or sets the display icon for this light when the state is off.
        /// </summary>
        [Display(Name = "Display Off Icon")]
        public string DisplayOffIcon { get; set; }

        /// <summary>
        /// Gets or sets the CSS color when the light is on.
        /// </summary>
        [Display(Name = "On Color")]
        public string OnColor { get; set; }

        /// <summary>
        /// Gets or sets the CSS color when the light is off.
        /// </summary>
        [Display(Name = "Off Color")]
        public string OffColor { get; set; }

        public override async Task OnClick(ServiceClient serviceClient)
        {
            // If this is a group, we can't use the toggle service, we have to know if it's on/off and call the opposite.
            if (EntityId.Split('.')[0].ToUpper() == "GROUP")
            {
                var stateClient = ClientFactory.GetClient<StatesClient>();
                var state = await stateClient.GetState(EntityId);
                var serviceName = state.State.ToUpper() == "ON" ? "turn_off" : "turn_on";
                await serviceClient.CallService("switch", serviceName, new { entity_id = EntityId });
            }
            else
            {
                await base.OnClick(serviceClient);
            }
        }
    }
}
