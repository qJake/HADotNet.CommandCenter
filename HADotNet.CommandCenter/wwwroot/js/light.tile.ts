/// <reference path="tile.ts" />

class LightTile extends Tile
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
        $(`#tile-${tile.name}`).find('span[value-icon]').addClass('mdi mdi-lightbulb');

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