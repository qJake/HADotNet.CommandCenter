using HADotNet.CommandCenter.Models.Config;
using HADotNet.CommandCenter.Models.Config.Tiles;

namespace HADotNet.CommandCenter.ViewModels
{
    public class TileWithLayoutViewModel
    {
        public BaseTile Tile { get; set; }
        public TileLayout Layout { get; set; }
        public LayoutSettings Settings { get; set; }
    }
}
