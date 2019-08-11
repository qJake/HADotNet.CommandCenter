interface ITile
{
    /** Gets or sets the name of the tile. */
    name: string;

    /** Gets or sets the associated entity ID. */
    entityId: string;

    /** Gets or sets the refresh interval for this tile. */
    refreshRate: number;
}