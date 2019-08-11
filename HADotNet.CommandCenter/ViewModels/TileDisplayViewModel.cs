using HADotNet.CommandCenter.Models.Config;
using System.Collections.Generic;

namespace HADotNet.CommandCenter.ViewModels
{
    public class TileDisplayViewModel
    {
        public LayoutSettings PageLayout { get; set; }
        public IEnumerable<TileWithLayoutViewModel> Tiles { get; set; }
    }
}
