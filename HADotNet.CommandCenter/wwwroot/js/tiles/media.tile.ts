/// <reference path="tile.ts" />

class MediaTile extends Tile
{
    private tile: IMediaTile;
    private state: IHAStateChangedData

    constructor(page: string, name: string, conn: signalR.HubConnection, private haConn: HAConnection)
    {
        super(page, name, conn, haConn, { canClick: true, canLoad: true });
    }

    public updateTile(t: ITile)
    {
        this.tile = <IMediaTile>t;
        super.updateTile(t);
        this.updateMediaImage();
        window.setInterval(() => this.updateMediaImage(), (this.tile.refreshRate > 0 ? this.tile.refreshRate : 1) * 1000);
    }

    public updateMediaImage(): void
    {
        if (!this.state?.new_state)
        {
            return;
        }
        let label = this.state.new_state.attributes["friendly_name"].toString();
        if (this.tile.overrideLabel)
        {
            label = this.tile.overrideLabel;
        }
        if (!this.state.new_state.attributes["entity_picture"])
        {
            label += ` (${this.state.new_state.state})`
        }
        $(`#tile-${this.tile.name}`).toggleClass('media-idle', this.state.new_state.attributes['media_title'] === 'Nothing playing' || ((this.state.new_state.state == 'paused' || this.state.new_state.state == 'idle') && !this.state.new_state.attributes["entity_picture"]));
        $(`#tile-${this.tile.name}`).find('span[value-name]').text(this.tile.showLabel ? label : '');
        $(`#tile-${this.tile.name}`).find('span[value-title]').text(this.tile.showTitle && this.state.new_state.attributes['media_title'] && this.state.new_state.attributes['media_title'] !== 'Nothing playing' ? this.state.new_state.attributes['media_title'].toString() : '');

        if (!this.state.new_state.attributes["entity_picture"])
        {
            return;
        }

        this.haConn.getMediaImage(this.tile.entityId)
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

    public updateState(s: IHAStateChangedData): void
    {
        this.state = s;
    }
}