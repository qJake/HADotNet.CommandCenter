/// <reference path="tile.ts" />
/// <reference path="blank.tile.ts" />
/// <reference path="label.tile.ts" />
/// <reference path="date.tile.ts" />
/// <reference path="state.tile.ts" />
/// <reference path="light.tile.ts" />
/// <reference path="switch.tile.ts" />
/// <reference path="person.tile.ts" />
/// <reference path="weather.tile.ts" />
/// <reference path="camera.tile.ts" />
/// <reference path="scene.tile.ts" />
/// <reference path="media.tile.ts" />
/// <reference path="navigation.tile.ts" />

type TypeMap = {
    [name: string]: any;
};

class TileMap
{
    static ClassMap: TypeMap = {
        'Blank': BlankTile,
        'Label': LabelTile,
        'Date': DateTile,
        'State': StateTile,
        'Light': LightTile,
        'Switch': SwitchTile,
        'Person': PersonTile,
        'Weather': WeatherTile,
        'Camera': CameraTile,
        'Scene': SceneTile,
        'Media': MediaTile,
        'Navigation': NavigationTile
    };
}