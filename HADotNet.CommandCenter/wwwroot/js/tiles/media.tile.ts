/// <reference path="tile.ts" />

class MediaTile extends Tile
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
        var mediaTile = <IMediaTile>tile;

        let label = state.attributes["friendly_name"].toString();
        if (tile.overrideLabel)
        {
            label = tile.overrideLabel;
        }
        $(`#tile-${tile.name}`).toggleClass('media-idle', state.attributes['media_title'] === 'Nothing playing' || ((state.state == 'paused' || state.state == 'idle') && !state.attributes["entity_picture"]));
        $(`#tile-${tile.name}`).find('span[value-name]').text(mediaTile.showLabel ? label : '');
        $(`#tile-${tile.name}`).find('span[value-title]').text(mediaTile.showTitle && state.attributes['media_title'] && state.attributes['media_title'] !== 'Nothing playing' ? state.attributes['media_title'].toString() : '');
        
        if (!this.config || !this.config.baseUrl)
        {
            if (this.firstLoadIgnored)
            {
                console.warn("Missing config.baseUrl, unable to render media display.", this.config);
            }
            else
            {
                this.firstLoadIgnored = true;
            }
            this.queueTileRefresh(tile);
        }
        else
        {
            if (!state.attributes["entity_picture"])
            {
                this.queueTileRefresh(tile, true);
                return;
            }

            const cacheBuster = Math.floor(Math.random() * Math.floor(99999999));

            let imageUrl = this.config.baseUrl + state.attributes["entity_picture"].toString() + "&_nocache=" + cacheBuster;

            Utils.preloadImage(imageUrl)
                .then(img =>
                {
                    let imageSize = mediaTile.imageCropMode.toLowerCase() === 'cover' || mediaTile.imageCropMode.toLowerCase() === 'contain'
                        ? mediaTile.imageCropMode.toLowerCase()
                        : '100% 100%';
                    let imagePosition = mediaTile.imageCropMode.toLowerCase() === 'cover' || mediaTile.imageCropMode.toLowerCase() === 'contain'
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