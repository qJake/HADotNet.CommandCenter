/// <reference path="tile.ts" />

class NavigationTile extends Tile
{
    private navTile: INavigationTile;

    constructor(page: string, name: string, conn: signalR.HubConnection, haConn: HAConnection)
    {
        super(page, name, conn, haConn, { canClick: true, canLoad: false });
    }

    public updateTile(tile: ITile): void
    {
        this.navTile = <INavigationTile>tile;

        $(`#tile-${tile.name}`).find('span[value-name]').text(this.navTile.label);
        $(`#tile-${tile.name}`).find('span[value-icon]').addClass(`mdi mdi-${this.navTile.displayIcon}`);
            
        super.updateTile();
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