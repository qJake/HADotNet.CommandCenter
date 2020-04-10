/// <reference path="tile.ts" />

class CameraTile extends Tile
{
    private tile: ICameraTile;

    constructor(page: string, name: string, conn: signalR.HubConnection, private haConn: HAConnection)
    {
        super(page, name, conn, haConn, { canClick: false, canLoad: false });
    }

    public updateTile(t: ITile)
    {
        this.tile = <ICameraTile>t;
        super.updateTile(t);
        this.updateCameraFeed();
        window.setInterval(() => this.updateCameraFeed(), (this.tile.refreshRate > 0 ? this.tile.refreshRate : 1) * 1000);
    }

    public updateCameraFeed(): void
    {
        this.haConn.getCameraImage(this.tile.entityId)
            .then(msg =>
            {
                let imageSize = this.tile.imageCropMode.toLowerCase() === 'cover' || this.tile.imageCropMode.toLowerCase() === 'contain'
                    ? this.tile.imageCropMode.toLowerCase()
                    : '100% 100%';
                let imagePosition = this.tile.imageCropMode.toLowerCase() === 'cover' || this.tile.imageCropMode.toLowerCase() === 'contain'
                    ? '50% 50%'
                    : '0 0';

                $(`#tile-${this.tile.name}`).css({
                    backgroundImage: `url('data:${msg.result.content_type};base64,${msg.result.content}')`,
                    backgroundRepeat: 'no-repeat',
                    backgroundPosition: imagePosition,
                    backgroundSize: imageSize
                });
            });
    }
}