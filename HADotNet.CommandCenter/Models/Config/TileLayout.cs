using Newtonsoft.Json;

namespace HADotNet.CommandCenter.Models.Config
{
    public class TileLayout
    {
        /// <summary>
        /// Gets or sets the tile name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the X position (Left) of the tile, in pixels.
        /// </summary>
        [JsonProperty("x")]
        public int XPos { get; set; }

        /// <summary>
        /// Gets or sets the Y position (Top) of the tile, in pixels.
        /// </summary>
        [JsonProperty("y")]
        public int YPos { get; set; }

        /// <summary>
        /// Gets or sets the ordered index of the tile.
        /// </summary>
        public int Index { get; set; }
    }
}
