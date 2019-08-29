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

enum WeatherTileEntities
{
    entityId = 'entityId',
    iconEntity = 'iconEntity',
    summaryEntity = 'summaryEntity',
    precipChanceEntity = 'precipChanceEntity',
    highTempEntity = 'highTempEntity',
    lowTempEntity = 'lowTempEntity',
    windSpeedEntity = 'windSpeedEntity'
}

interface IWeatherTile extends ITile
{

    /** Gets or sets the entity name for the icon value. */
    iconEntity: string;

    /** Gets or sets the entity name for the summary value. */
    summaryEntity: string;
    
    /** Gets or sets the entity name for the precipitation chance value. */
    precipChanceEntity: string;
    
    /** Gets or sets the entity name for the high temp value. */
    highTempEntity: string;
    
    /** Gets or sets the entity name for the low temp value. */
    lowTempEntity: string;
    
    /** Gets or sets the entity name for the wind speed value. */
    windSpeedEntity: string;
}