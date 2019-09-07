/// <reference path="tile.ts" />

class SceneTile extends Tile
{
    public updateState(tile: ITile, state: IEntityState): void
    {
        var sceneTile = <ISceneTile>tile;

        let label = state.attributes["friendly_name"].toString();
        if (tile.overrideLabel)
        {
            label = tile.overrideLabel;
        }

        $(`#tile-${tile.name}`).find('span[value-name]').text(label);
        $(`#tile-${tile.name}`).find('span[value-icon]').addClass(`mdi mdi-${sceneTile.displayIcon || 'filmstrip'}`);
        $(`#tile-${tile.name} .value`).css('color', sceneTile.iconColor)
            
        super.updateState();
    }
}