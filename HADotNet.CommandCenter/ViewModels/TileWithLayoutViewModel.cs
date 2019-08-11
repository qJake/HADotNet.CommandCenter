using HADotNet.CommandCenter.Models.Config;
using HADotNet.CommandCenter.Models.Config.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.ViewModels
{
    public class TileWithLayoutViewModel
    {
        public BaseTile Tile { get; set; }
        public TileLayout Layout { get; set; }
    }
}
