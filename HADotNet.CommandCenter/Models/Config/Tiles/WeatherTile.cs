using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        public string GetJsonEntityList()
        {
            return JsonConvert.SerializeObject(new List<string>
            {
                EntityId,
                IconEntity,
                SummaryEntity,
                PrecipChanceEntity,
                HighTempEntity,
                LowTempEntity,
                WindSpeedEntity,
                WindDirectionEntity
            });
        }
    }
}
