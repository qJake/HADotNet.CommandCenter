interface ITile
{
    /** Gets or sets the name of the tile. */
    name: string;

    /** Gets or sets the associated entity ID. */
    entityId: string;

    /** Gets or sets the refresh interval for this tile. */
    refreshRate: number;

    /** Gets or sets the override name for the tile. */
    overrideLabel?: string;
}

interface ISwitchTile extends ITile
{
    /** Gets or sets the display icon name. */
    displayIcon: string;

    /** Gets or sets the display icon name. */
    displayOffIcon: string;

    /** Gets or sets the "off" color. */
    offColor: string;

    /** Gets or sets the "on" color. */
    onColor: string;
}

interface ILightTile extends ISwitchTile
{
    // Reserved for possible future light-specific properties such as brightness %.
}