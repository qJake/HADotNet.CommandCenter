/// <reference path="tile.ts" />

class SwitchTile extends Tile
{
    private tile: ISwitchTile;

    public updateTile(t: ITile)
    {
        this.tile = <ISwitchTile>t;
        super.updateTile(t);
    }

    public updateState(state: IHAStateChangedData): void
    {
        //console.log("State received for: " + tile.name, state);
        let label = state.new_state.attributes["friendly_name"].toString();
        if (this.tile.overrideLabel)
        {
            label = this.tile.overrideLabel;
        }
        $(`#tile-${this.tile.name}`).find('span[value-name]').text(label);
        $(`#tile-${this.tile.name}`).find('span[value-icon]')
            .removeClass(`mdi-${this.tile.displayIcon} mdi-${this.tile.displayOffIcon}`)
            .addClass(`mdi mdi-${state.new_state.state.toLowerCase() === "on" ? Utils.resolveIcon(state.new_state.attributes["icon"], this.tile.displayIcon) : Utils.resolveIcon(state.new_state.attributes["icon"], this.tile.displayOffIcon || this.tile.displayIcon)}`);

        // TODO: Add custom on/off state keywords
        $(`#tile-${this.tile.name}`)
            .find('span[value-icon]')
            .removeClass("state-off state-on")
            .addClass(state.new_state.state.toLowerCase() === "on" ? "state-on" : "state-off");

        if (this.tile.onColor && state.new_state.state.toLowerCase() === "on")
        {
            $(`#tile-${this.tile.name} .value`).css('color', this.tile.onColor)
        }
        if (this.tile.offColor && state.new_state.state.toLowerCase() !== "on")
        {
            $(`#tile-${this.tile.name} .value`).css('color', this.tile.offColor)
        }
            
        super.updateState(state);
    }
}