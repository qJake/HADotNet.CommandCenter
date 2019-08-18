/// <reference path="tile.ts" />

class LabelTile extends Tile
{
    constructor(name: string, conn: signalR.HubConnection)
    {
        super(name, conn, false);
    }
}