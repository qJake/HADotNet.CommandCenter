﻿/// <reference path="tile.ts" />

class NavigationTile extends Tile
{
    private navTile: INavigationTile;

    public updateTileState(tile: ITile): void
    {
        this.navTile = <INavigationTile>tile;

        $(`#tile-${tile.name}`).find('span[value-name]').text(this.navTile.label);
        $(`#tile-${tile.name}`).find('span[value-icon]').addClass(`mdi mdi-${this.navTile.displayIcon}`);
            
        super.updateState();
    }

    protected onClick(): Promise<any>
    {
        switch (this.navTile.mode.toLowerCase().trim())
        {
            case 'home':
                window.location.href = '/d/';
                return;
            case 'refresh':
                window.location.reload();
                return;
            case 'nav':
                window.location.href = `/d/${this.navTile.target}`;
                return;
        }
    }
}