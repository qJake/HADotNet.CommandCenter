/// <reference path="tile.ts" />

class LabelTile extends Tile
{
    constructor(page: string, name: string, conn: signalR.HubConnection)
    {
        super(page, name, conn, false);
    }
}