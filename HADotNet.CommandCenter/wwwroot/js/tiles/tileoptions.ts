interface ITileOptions
{
    /** Determines if this tile is loadable via the SignalR hub. */
    canLoad?: boolean;

    /** Determines if this tile is clickable and will call the appropriate HA action. */
    canClick?: boolean;
}