interface IHaccConfig
{
    /** Gets or sets the base URL for the system. */
    baseUrl: string;
}

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

interface IStateTile extends ITile
{
    /** Gets or sets a value indicating if decimal values should be rounded. */
    roundDecimals: boolean;
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

interface ISceneTile extends ITile
{
    /** Gets or sets the display icon name. */
    displayIcon: string;

    /** Gets or sets the "off" color. */
    iconColor: string;
}

enum WeatherTileEntities
{
    entityId = 'entityId',
    iconEntity = 'iconEntity',
    summaryEntity = 'summaryEntity',
    precipChanceEntity = 'precipChanceEntity',
    highTempEntity = 'highTempEntity',
    lowTempEntity = 'lowTempEntity',
    windSpeedEntity = 'windSpeedEntity',
    windDirectionEntity = 'windDirectionEntity'
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
    
    /** Gets or sets the entity name for the wind direction value. */
    windDirectionEntity: string;

    /** Gets or sets a value indicating if wind speed should be rounded. */
    roundWindSpeed: boolean;
}

interface ICameraTile extends ITile
{
    /** Gets or sets the refresh rate, in seconds, of the camera image. */
    refreshRate: number;

    /** Gets or sets the image crop mode. */
    imageCropMode: string;
}

interface IMediaTile extends ITile
{
    /** Gets or sets the refresh rate, in seconds, of the camera image. */
    refreshRate: number;

    /** Gets or sets the image crop mode. */
    imageCropMode: string;

    /** Whether or not to show the media title. */
    showTitle: boolean;

    /** Whether or not to show the entity name. */
    showLabel: boolean;
}

interface INavigationTile extends ITile
{
    /** Gets or sets the refresh rate, in seconds, of the camera image. */
    mode: string;

    /** Gets or sets the image crop mode. */
    target: string;

    /** Whether or not to show the media title. */
    label: string;

    /** Whether or not to show the entity name. */
    displayIcon: string;
}