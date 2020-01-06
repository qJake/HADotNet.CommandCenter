/// <reference path="tile.ts" />

class LabelTile extends Tile
{
    constructor(page: string, name: string, conn: signalR.HubConnection, haConn: HAConnection)
    {
        super(page, name, conn, haConn, false);
    }
}