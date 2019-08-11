/// <reference path="models/models.ts" />
/// <reference path="typings/window-options.d.ts" />
/// <reference path="typings/draggabilly.d.ts" />
/// <reference path="typings/packery.d.ts" />
/// <reference path="typings/packery.jquery.d.ts" />
/// <reference path="../../node_modules/@aspnet/signalr/dist/esm/index.d.ts" />

type TilePos = {
    x: number,
    y: number,
    index: number,
    name: string
};

class CommandCenter
{
    private pk: Packery;
    private pageIsDirty: boolean;
    private tileConn: signalR.HubConnection;
    private tiles: Tile[];

    constructor()
    {
        this.tiles = [];
        $(() => this.init());
    }

    private init(): void
    {
        window.ccOptions.mode == PageMode.Admin
            ? this.initAdmin()
            : this.initUser();
    }

    private initAdmin(): void
    {
        $(window).on('beforeunload', e =>
        {
            if (this.pageIsDirty && $((e.target as any).activeElement).prop('type') !== 'submit')
            {
                return 'You have unsaved changes. Are you sure you want to leave?';
            }
        });

        $('.ui.accordion').accordion();
        $('.ui.dropdown').dropdown({ fullTextSearch: true });

        this.pk = new Packery('.preview-layout-grid', {
            itemSelector: '.preview-layout-item',
            columnWidth: '.preview-layout-item',
            rowHeight: '.preview-layout-item',
            gutter: window.ccOptions.tilePreviewPadding
        });
        this.pk.layout();

        this.pk.on('layoutComplete', () => this.writeItemLayout());
        this.pk.on('dragItemPositioned', () =>
        {
            // Things get kinda glitchy if we don't add a slight pause
            setTimeout(() =>
            {
                this.pk.layout();
                this.writeItemLayout();
                this.pageIsDirty = true;
            }, 25);
        });
        this.writeItemLayout();

        if (typeof Draggabilly === 'function')
        {
            $('.preview-layout-item').each((_, e) => this.pk.bindDraggabillyEvents(new Draggabilly(e)));
        }
        else
        {
            console.warn("Draggabilly is not available - drag and drop interface will not work.");
        }
    }

    private initUser(): void
    {
        this.tileConn = new signalR.HubConnectionBuilder().withUrl('/hubs/tile').build();
        this.tileConn.start().then(() =>
        {
            $('.tiles .tile').each((_, e) =>
            {
                this.tiles.push(new Tile($(e).data('tile-name'), this.tileConn));
            });
        });
    }

    private writeItemLayout(): void
    {
        var positions: TilePos[] = [];
        var tiles = this.pk.getItemElements();

        for (let i = 0; i < tiles.length; i++)
        {
            let $tile = $(tiles[i]);
            positions.push({
                index: i,
                x: parseInt($tile.css('left').replace('px', '')),
                y: parseInt($tile.css('top').replace('px', '')),
                name: $tile.data('tile-name')
            });
        }

        $('#layout-serialized').val(JSON.stringify(positions));
    }
}
var __app = new CommandCenter();