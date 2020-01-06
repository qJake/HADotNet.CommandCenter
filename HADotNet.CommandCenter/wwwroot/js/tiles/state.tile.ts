/// <reference path="tile.ts" />

class StateTile extends Tile
{
    private tile: IStateTile;

    public updateTile(t: ITile)
    {
        this.tile = <IStateTile>t;
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

        let value = this.tile.roundDecimals ? parseInt(state.new_state.state).toString() : state.new_state.state;
        if (state.new_state.attributes["unit_of_measurement"])
        {
            value += state.new_state.attributes["unit_of_measurement"].toString();
        }
        $(`#tile-${this.tile.name}`).find('span[value-state]').text(value);
    }
}