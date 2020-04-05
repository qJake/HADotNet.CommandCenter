/// <reference path="tile.ts" />

class DateTile extends Tile
{
    private refreshTimer: number;

    constructor(page: string, name: string, conn: signalR.HubConnection, haConn: HAConnection)
    {
        super(page, name, conn, haConn, { canClick: false, canLoad: true });
        this.refreshTimer = null;
    }

    protected updateDateTime(tile: ITile, date: string, time: string): void
    {
        $(`#tile-${tile.name}`).find('span[value-date]').text(date);
        $(`#tile-${tile.name}`).find('span[value-time]').text(time);

        super.updateDateTime();

        if (!this.refreshTimer)
        {
            this.refreshTimer = window.setTimeout((t: DateTile) =>
            {
                t.refreshTimer = null;
                this.requestState();
            }, 30 * 1000, this);
        }
    }
}