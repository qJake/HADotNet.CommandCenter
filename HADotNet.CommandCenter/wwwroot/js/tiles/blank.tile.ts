﻿/// <reference path="tile.ts" />

class BlankTile extends Tile
{
    constructor(page: string, name: string, conn: signalR.HubConnection, haConn: HAConnection)
    {
        super(page, name, conn, haConn, { canClick: false, canLoad: false });
    }
}