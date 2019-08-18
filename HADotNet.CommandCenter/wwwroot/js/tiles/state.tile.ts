/// <reference path="tile.ts" />

class StateTile extends Tile
{
    public updateState(tile: ITile, state: IEntityState): void
    {
        //console.log("State received for: " + tile.name, state);
        let label = state.attributes["friendly_name"].toString();
        if (tile.overrideLabel)
        {
            label = tile.overrideLabel;
        }
        $(`#tile-${tile.name}`).find('span[value-name]').text(label);

        let value = state.state;
        if (state.attributes["unit_of_measurement"])
        {
            value += state.attributes["unit_of_measurement"].toString();
        }
        $(`#tile-${tile.name}`).find('span[value-state]').text(value);

        super.updateState();

        if (tile.refreshRate > 0)
        {
            setTimeout(() =>
            {
                this.requestState(1000);
            }, tile.refreshRate * 1000);
        }
    }
}