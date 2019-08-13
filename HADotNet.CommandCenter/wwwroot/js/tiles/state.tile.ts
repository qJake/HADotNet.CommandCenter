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

        this.el.removeClass("tile-loading");

        if (tile.refreshRate > 0)
        {
            setTimeout(() =>
            {
                this.requestState();
            }, tile.refreshRate * 1000);
        }
    }
}