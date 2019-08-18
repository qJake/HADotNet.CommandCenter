/// <reference path="tile.ts" />

class SwitchTile extends Tile
{
    public updateState(tile: ITile, state: IEntityState): void
    {
        var lightTile = <ISwitchTile>tile;

        //console.log("State received for: " + tile.name, state);
        let label = state.attributes["friendly_name"].toString();
        if (tile.overrideLabel)
        {
            label = tile.overrideLabel;
        }
        $(`#tile-${tile.name}`).find('span[value-name]').text(label);
        $(`#tile-${tile.name}`).find('span[value-icon]')
            .removeClass(`mdi-${lightTile.displayIcon} mdi-${lightTile.displayOffIcon}`)
            .addClass(`mdi mdi-${state.state.toLowerCase() === "on" ? Utils.resolveIcon(state.attributes["icon"], lightTile.displayIcon) : Utils.resolveIcon(state.attributes["icon"], lightTile.displayOffIcon || lightTile.displayIcon)}`);

        // TODO: Add custom on/off state keywords
        $(`#tile-${tile.name}`).find('span[value-icon]').css('color', `${state.state.toLowerCase() === "on" ? lightTile.onColor : lightTile.offColor}`);
            
        super.updateState();

        if (tile.refreshRate > 0)
        {
            setTimeout(() =>
            {
                this.requestState(2000);
            }, tile.refreshRate * 1000);
        }
    }
}