/// <reference path="../../node_modules/@aspnet/signalr/dist/esm/index.d.ts" />

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
        this.requestState();
    }

    abstract updateState(tile: ITile, state: IEntityState): void;

    protected requestState(): void
    {
        this.el.addClass("tile-loading");
        this.conn.invoke('RequestTileState', this.name);
    }
}