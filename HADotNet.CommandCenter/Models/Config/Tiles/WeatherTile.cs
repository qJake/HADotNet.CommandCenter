using HADotNet.Core.Clients;
using HADotNet.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Models.Config.Tiles
{
    [TileType("weather")]
    [TileIcon(TileIconType.Material, "weather-partly-cloudy")]
    public class WeatherTile : BaseEntityTile
    {
        [Display(Name = "Icon Entity")]
        public string IconEntity { get; set; }

        [Display(Name = "Summary Entity")]
        public string SummaryEntity { get; set; }

        [Display(Name = "Precipitation Chance Entity")]
        public string PrecipChanceEntity { get; set; }

        [Display(Name = "High Temp Entity")]
        public string HighTempEntity { get; set; }

        [Display(Name = "Low Temp Entity")]
        public string LowTempEntity { get; set; }

        [Display(Name = "Wind Speed Entity")]
        public string WindSpeedEntity { get; set; }

        [Display(Name = "Round Wind Speed")]
        public bool RoundWindSpeed { get; set; }

        [Display(Name = "Wind Direction Entity")]
        public string WindDirectionEntity { get; set; }

        public override StateObject StateManipulator(StateObject state)
        {
            if (RoundWindSpeed && state.EntityId == WindSpeedEntity && decimal.TryParse(state.State, out var value))
            {
                state.State = Math.Round(value, 0).ToString();
            }

            return state;
        }
    }
}
