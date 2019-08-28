using HADotNet.CommandCenter.Models.Config;
using HADotNet.CommandCenter.Models.Config.Themes;
using System.Collections.Generic;

namespace HADotNet.CommandCenter.ViewModels
{
    public class TileDisplayViewModel
    {
        public LayoutSettings PageLayout { get; set; }
        public Theme Theme { get; set; }
        public IEnumerable<TileWithLayoutViewModel> Tiles { get; set; }
    }
}
