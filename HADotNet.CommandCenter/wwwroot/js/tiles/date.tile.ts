/// <reference path="tile.ts" />

class DateTile extends Tile
{
    protected updateState(tile: ITile, date: string, time: string): void
    {
        $(`#tile-${tile.name}`).find('span[value-date]').text(date);
        $(`#tile-${tile.name}`).find('span[value-time]').text(time);

        super.updateState();

        setTimeout(() =>
        {
            this.requestState(9500);
        }, 10000);
    }
}