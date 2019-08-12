using System;

namespace HADotNet.CommandCenter.Models.Config.Tiles
{
    public class TileIconAttribute : Attribute
    {
        public TileIconType IconType { get; set; }
        public string IconName { get; set; }

        /// <summary>
        /// Initializes the tile icon attribute with the specified icon type and name.
        /// </summary>
        /// <param name="type">The icon library / type.</param>
        /// <param name="name">The icon name. Do not include any library prefixes (e.g. for Semantic for "home icon", enter "home"; for Material for "mdi mdi-home", enter "home").</param>
        public TileIconAttribute(TileIconType type, string name)
        {
            IconType = type;
            IconName = name;
        }
    }
}
