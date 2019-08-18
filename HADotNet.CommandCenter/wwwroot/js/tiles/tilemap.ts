/// <reference path="tile.ts" />
/// <reference path="blank.tile.ts" />
/// <reference path="label.tile.ts" />
/// <reference path="date.tile.ts" />
/// <reference path="state.tile.ts" />
/// <reference path="light.tile.ts" />
/// <reference path="switch.tile.ts" />
/// <reference path="person.tile.ts" />

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
        'Person': PersonTile
    };
}