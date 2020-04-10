/// <reference path="tile.ts" />

class SwitchTile extends Tile
{
    private tile: ISwitchTile;

    constructor(page: string, name: string, conn: signalR.HubConnection, haConn: HAConnection)
    {
        super(page, name, conn, haConn, { canClick: true, canLoad: false });
    }

    public updateTile(t: ITile)
    {
        this.tile = <ISwitchTile>t;
        super.updateTile(t);
    }

    public updateState(state: IHAStateChangedData): void
    {
        if (this.tile == null) return;

        //console.log("State received for: " + tile.name, state);
        let label = state.new_state.attributes["friendly_name"].toString();
        if (this.tile?.overrideLabel)
        {
            label = this.tile.overrideLabel;
        }
        $(`#tile-${this.tile.name}`).find('span[value-name]').text(label);
        $(`#tile-${this.tile.name}`).find('span[value-icon]')
            .removeClass(`mdi-${this.tile.displayIcon} mdi-${this.tile.displayOffIcon}`)
            .addClass(`mdi mdi-${this.isOnState(state.new_state.state) ? Utils.resolveIcon(state.new_state.attributes["icon"], this.tile.displayIcon) : Utils.resolveIcon(state.new_state.attributes["icon"], this.tile.displayOffIcon || this.tile.displayIcon)}`);

        // TODO: Add custom on/off state keywords
        $(`#tile-${this.tile.name}`)
            .find('span[value-icon]')
            .removeClass("state-off state-on")
            .addClass(this.isOnState(state.new_state.state) ? "state-on" : "state-off");

        if (this.tile.onColor && this.isOnState(state.new_state.state))
        {
            $(`#tile-${this.tile.name} .value`).css('color', this.tile.onColor)
        }
        if (this.tile.offColor && !this.isOnState(state.new_state.state))
        {
            $(`#tile-${this.tile.name} .value`).css('color', this.tile.offColor)
        }
            
        super.updateState(state);
    }

    private isOnState(state: string): boolean
    {
        return state.toLowerCase() === 'on' || state.toLowerCase() === 'open' || state.toLowerCase() === 'detected';
    }
}