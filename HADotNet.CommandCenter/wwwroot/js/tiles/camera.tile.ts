/// <reference path="tile.ts" />

class CameraTile extends Tile
{
    // Kind of a hack/workaround since it's not easy to control the order (requestTileState comes before requestConfig so it's missing the first time around).
    private firstLoadIgnored = false;

    constructor(page: string, name: string, conn: signalR.HubConnection)
    {
        super(page, name, conn, true);

        this.requestConfig();
    }

    public updateState(tile: ITile, state: IEntityState): void
    {
        var cameraTile = <ICameraTile>tile;

        if (!this.config || !this.config.baseUrl)
        {
            if (this.firstLoadIgnored)
            {
                console.warn("Missing config.baseUrl, unable to render camera display.", this.config);
            }
            else
            {
                this.firstLoadIgnored = true;
            }
            this.queueTileRefresh(tile);
        }
        else
        {
            const cacheBuster = Math.floor(Math.random() * Math.floor(99999999));

            let imageUrl = this.config.baseUrl + state.attributes["entity_picture"].toString() + "&_nocache=" + cacheBuster;

            Utils.preloadImage(imageUrl)
                .then(img =>
                {
                    let imageSize = cameraTile.imageCropMode.toLowerCase() === 'cover' || cameraTile.imageCropMode.toLowerCase() === 'contain'
                        ? cameraTile.imageCropMode.toLowerCase()
                        : '100% 100%';
                    let imagePosition = cameraTile.imageCropMode.toLowerCase() === 'cover' || cameraTile.imageCropMode.toLowerCase() === 'contain'
                        ? '50% 50%'
                        : '0 0';

                    $(`#tile-${tile.name}`).css({
                        backgroundImage: `url('${img}')`,
                        backgroundRepeat: 'no-repeat',
                        backgroundPosition: imagePosition,
                        backgroundSize: imageSize
                    });
                })
                .finally(() => this.queueTileRefresh(tile, true));
        }
    }

    private queueTileRefresh(tile: ITile, clearLoading?: boolean): void
    {
        if (clearLoading)
        {
            super.updateState();
        }

        if (tile.refreshRate > 0)
        {
            setTimeout(() =>
            {
                this.requestState((tile.refreshRate * 1000) - 100);
            }, tile.refreshRate * 1000);
        }
    }
}