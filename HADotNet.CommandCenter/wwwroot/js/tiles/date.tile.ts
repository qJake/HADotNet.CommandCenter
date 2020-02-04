/// <reference path="tile.ts" />

class DateTile extends Tile
{
    constructor(page: string, name: string, conn: signalR.HubConnection, haConn: HAConnection)
    {
        super(page, name, conn, haConn, { canClick: false, canLoad: true });
    }

    protected updateDateTime(tile: ITile, date: string, time: string): void
    {
        $(`#tile-${tile.name}`).find('span[value-date]').text(date);
        $(`#tile-${tile.name}`).find('span[value-time]').text(time);

        super.updateDateTime();

        setTimeout(() =>
        {
            this.requestState(9500);
        }, 10000);
    }
}