using HADotNet.CommandCenter.Models.Config.Tiles;
using HADotNet.CommandCenter.Utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HADotNet.CommandCenter.Models.Config.Pages
{
    /// <summary>
    /// Represents a dashboard page.
    /// </summary>
    public class Page
    {
        /// <summary>
        /// Gets or sets the name for this page. This name is used to navigate to the page.
        /// </summary>
        [Required]
        [Display(Name = "Page ID")]
        [RegularExpression(@"^[a-zA-Z0-9][a-zA-Z0-9-]*$", ErrorMessage = "The name can only be A-Z, 0-9, and hyphens, and must start with a letter or number.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the human friendly description for this page.
        /// </summary>
        [Required]
        [Display(Name = "Page Name")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets whether or not this page is the default page. Only one page can be the default.
        /// </summary>
        [Display(Name = "Set as Default Page")]
        public bool IsDefaultPage { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds after which this page will automatically return to the default page. If set to 0, auto return will be disabled.
        /// </summary>
        [Display(Name = "Navigation Auto Return (secs)")]
        public int AutoReturnSeconds { get; set; }

        /// <summary>
        /// Gets or sets the list of tiles.
        /// </summary>
        public List<BaseTile> Tiles { get; set; }

        /// <summary>
        /// Gets or sets the main tile layout.
        /// </summary>
        public List<TileLayout> TileLayout { get; set; }

        /// <summary>
        /// Gets or sets the layout settings for this page.
        /// </summary>
        public LayoutSettings LayoutSettings { get; set; }
    }
}
