﻿using HADotNet.Core;
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

        /// <summary>
        /// Handle the click event differently for switches.
        /// </summary>
        /// <remarks>Switches are weird... some of them don't like the homeassistant.toggle service. So we do our own logic.</remarks>
        public override async Task OnClick(ServiceClient serviceClient)
        {
            var serviceDomain = EntityId.Split('.')[0].ToLower();
            if (serviceDomain.ToUpper() == "GROUP")
            {
                serviceDomain = "homeassistant";
            }

            var stateClient = ClientFactory.GetClient<StatesClient>();
            var state = await stateClient.GetState(EntityId);
            var serviceName = 
                    state.State.ToUpper() == "OFF"
                || state.State.ToUpper() == "UNAVAILABLE"
                || state.State.ToUpper() == "NONE"
                || state.State.ToUpper() == "STANDBY"
                || state.State.ToUpper() == "IDLE"
                ? "turn_on" : "turn_off";

            await serviceClient.CallService(serviceDomain, serviceName, new { entity_id = EntityId });
        }
    }
}
