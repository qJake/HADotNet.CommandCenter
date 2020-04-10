/// <reference path="tile.ts" />

class SceneTile extends Tile
{
    private tile: ISceneTile;

    constructor(page: string, name: string, conn: signalR.HubConnection, haConn: HAConnection)
    {
        super(page, name, conn, haConn, { canClick: true, canLoad: true });
    }

    public updateTile(t: ITile)
    {
        this.tile = <ISceneTile>t;
        super.updateTile(t);
    }

    public updateState(state: IHAStateChangedData): void
    {
        let label = state.new_state.attributes["friendly_name"].toString();
        if (this.tile.overrideLabel)
        {
            label = this.tile.overrideLabel;
        }

        $(`#tile-${this.tile.name}`).find('span[value-name]').text(label);
        $(`#tile-${this.tile.name}`).find('span[value-icon]').addClass(`mdi mdi-${this.tile.displayIcon || 'filmstrip'}`);
        $(`#tile-${this.tile.name} .value`).css('color', this.tile.iconColor)
    }
}