/// <reference path="tile.ts" />

class PersonTile extends Tile
{
    public updateState(tile: ITile, state: IEntityState): void
    {
        //console.log("State received for: " + tile.name, state);
        let picture = state.attributes['entity_picture'] ? state.attributes['entity_picture'].toString() : '';
        let location = state.state.replace('_', ' ');
        let label = state.attributes['friendly_name'].toString();
        if (tile.overrideLabel)
        {
            label = tile.overrideLabel;
        }

        let isHome = location.toLowerCase() === 'home';

        $(`#tile-${tile.name}`).find('span[value-name]').text(label);
        $(`#tile-${tile.name}`).find('span[value-location]').text(location);
        $(`#tile-${tile.name}`).find('span[value-picture]').css('background-image', `url(${picture})`).removeClass('bw');
        if (!isHome)
        {
            $(`#tile-${tile.name}`).find('span[value-picture]').addClass('bw');
        }
            
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