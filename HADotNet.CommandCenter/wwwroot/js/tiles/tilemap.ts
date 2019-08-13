/// <reference path="tile.ts" />
/// <reference path="blank.tile.ts" />
/// <reference path="date.tile.ts" />
/// <reference path="state.tile.ts" />
/// <reference path="light.tile.ts" />

type TypeMap = {
    [name: string]: any;
};

class TileMap
{
    static ClassMap: TypeMap = {
        'Blank': BlankTile,
        'Date': DateTile,
        'State': StateTile,
        'Light': LightTile
    };
}