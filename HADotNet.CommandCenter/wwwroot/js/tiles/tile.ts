/// <reference path="../../../node_modules/@aspnet/signalr/dist/esm/index.d.ts" />

abstract class Tile
{
    protected el: JQuery<HTMLElement>;

    constructor(protected name: string, protected conn: signalR.HubConnection)
    {
        this.el = $(`.tiles .tile[data-tile-name="${name}"]`);

        conn.on('SendTileState', (t, s) =>
        {
            if (name == (t as ITile).name)
            {
                this.updateState(t, s);
            }
        });
        conn.on('SendWarning', msg => console.warn(msg));
        conn.on('SendDateTime', (tile, d, t) =>
        {
            if (name == (tile as ITile).name)
            {
                this.updateDateTime(tile, d, t);
            }
        });
        this.requestState();
    }

    protected updateState(tile: ITile, state: IEntityState): void
    {

    }

    protected updateDateTime(tile: ITile, date: string, time: string): void
    {

    }

    protected requestState(): void
    {
        this.el.addClass("tile-loading");
        this.conn.invoke('RequestTileState', this.name);
    }
}