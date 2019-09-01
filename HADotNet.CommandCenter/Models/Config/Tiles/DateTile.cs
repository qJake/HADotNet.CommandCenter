using System.ComponentModel.DataAnnotations;

namespace HADotNet.CommandCenter.Models.Config.Tiles
{
    [TileType("date")]
    [TileIcon(TileIconType.Material, "clock-outline")]
    public class DateTile : BaseTile
    {
        [Display(Name = "Date Format String")]
        public string DateFormatString { get; set; }

        [Display(Name = "Time Format String")]
        public string TimeFormatString { get; set; }

        [Display(Name = "Time Zone")]
        public string TimeZoneId { get; set; }
    }
}
