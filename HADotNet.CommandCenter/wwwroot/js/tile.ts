/// <reference path="../../node_modules/@aspnet/signalr/dist/esm/index.d.ts" />

class Tile
{
    constructor(private name: string, private conn: signalR.HubConnection)
    {
        conn.on('SendTileState', (t, s) =>
        {
            if (name == (t as ITile).name)
            {
                this.updateState(t, s);
            }
        });
        this.requestState();
    }

    private updateState(tile: ITile, state: IEntityState): void
    {
        //console.log("State received for: " + tile.name, state);

        $(`#tile-${tile.name}`).find('span[value-name]').text(state.attributes["friendly_name"].toString());

        let value = state.state;
        if (state.attributes["unit_of_measurement"])
        {
            value += state.attributes["unit_of_measurement"].toString();
        }

        $(`#tile-${tile.name}`).find('span[value-state]').text(value);

        if (tile.refreshRate > 0)
        {
            setTimeout(() =>
            {
                this.requestState();
            }, tile.refreshRate * 1000);
        }
    }

    private requestState(): void
    {
        this.conn.invoke('RequestTileState', this.name);
    }
}