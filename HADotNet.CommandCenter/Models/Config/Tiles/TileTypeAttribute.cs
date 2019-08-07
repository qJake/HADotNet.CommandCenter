using System;

namespace HADotNet.CommandCenter.Models.Config.Tiles
{
    /// <summary>
    /// Specifies that this class is a type of tile, with the specified <see cref="Name" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TileTypeAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the type name of this tile class.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Marks this class as being a type of tile, with the specified <paramref name="name" />.
        /// </summary>
        /// <param name="name">The type name for this tile.</param>
        public TileTypeAttribute(string name)
        {
            Name = name;
        }
    }
}
