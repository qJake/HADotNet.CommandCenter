/// <reference path="models/models.ts" />
/// <reference path="typings/window-options.d.ts" />
/// <reference path="typings/draggabilly.d.ts" />
/// <reference path="typings/packery.d.ts" />
/// <reference path="typings/packery.jquery.d.ts" />
/// <reference path="../../node_modules/@aspnet/signalr/dist/esm/index.d.ts" />
/// <reference path="tiles/tilemap.ts" />
/// <reference path="PageFunctions.ts" />

type TilePos = {
    x: number,
    y: number,
    index: number,
    name: string
};

class CommandCenter
{
    private conn: HAConnection;

    private pk: Packery;
    private pageIsDirty: boolean;
    private tileConn: signalR.HubConnection;
    private tiles: Tile[];
    private initHandle: number;

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

        this.initializeMdiPreview();
        this.initializeColorPreview();
        this.initializeNightlyRefresh();
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

        $('#importTheme, #importConfig').click(() =>
        {
            if (confirm('WARNING: This will OVERWRITE your current settings. Export first if you want to save what you have now! Continue?'))
            {
                $('#importBrowser').click();
            }
        });

        $('#importBrowser').change(() =>
        {
            $('#importForm').submit();
        });

        $('#resetConfig').click(e =>
        {
            if (!confirm("WARNING: This will COMPLETELY RESET your HACC installation and PERMANENTLY DELETE all of your tiles, themes, and settings. Are you sure you want to reset your config?"))
            {
                e.preventDefault();
                return false;
            }
            return true;
        })

        $('.ui.accordion').accordion();
        $('.ui.checkbox').checkbox();
        $('.ui.dropdown').not('.no-placeholder').dropdown({ fullTextSearch: true });
        $('.ui.no-placeholder.dropdown').dropdown({ placeholder: false });

        // Font dropdown with real font previews.
        $('#Page_PageFontFace option').each(function (_, e)
        {
            $(e).parent().siblings('.menu').find('.item[data-value="' + $(e).text() + '"]').css('font-family', $(e).text());
        });

        // Only init Packery stuff if we detect we have the preview grid on the page
        if ($('.preview-layout-grid').length)
        {
            $('#auto-layout').click(() =>
            {
                if (confirm('This will reset the layout for this page and attempt to automatically arrange all tiles evenly.\n\nYour tiles will all probably change locations. You have been warned. :)\n\nAre you sure you want to reset the layout?'))
                {
                    this.pk.layout();
                }
            });

            // For some reason Draggabilly takes the first element as the grid size, so inject a temporary invisible "fake" one
            $('.preview-layout-grid').prepend(`<div class="preview-layout-item" style="opacity: 0; position: absolute; top: ${window.ccOptions.tilePreviewPadding}px; left: ${window.ccOptions.tilePreviewPadding}px; width: ${window.ccOptions.tilePreviewSize}px; height: ${window.ccOptions.tilePreviewSize}px;" id="grid__tmp"></div>`);

            if (window.ccOptions)
            {
                this.pk = new Packery('.preview-layout-grid', {
                    itemSelector: '.preview-layout-item',
                    columnWidth: window.ccOptions.tilePreviewSize,
                    rowHeight: window.ccOptions.tilePreviewSize,
                    gutter: window.ccOptions.tilePreviewPadding,
                    initLayout: false
                });
            }
            else
            {
                this.pk = new Packery('.preview-layout-grid', {
                    itemSelector: '.preview-layout-item',
                    columnWidth: '.preview-layout-item',
                    rowHeight: '.preview-layout-item',
                    gutter: window.ccOptions.tilePreviewPadding,
                    initLayout: false
                });
            }

            this.pk.on('layoutComplete', () => this.writeItemLayout());
            this.pk.on('dragItemPositioned', () =>
            {
                // Things get kinda glitchy if we don't add a slight pause
                setTimeout(() =>
                {
                    this.writeItemLayout();
                    this.pageIsDirty = true;
                }, 25);
            });
            this.writeItemLayout();

            if (typeof Draggabilly === 'function')
            {
                $('.preview-layout-item').each((_, e) => this.pk.bindDraggabillyEvents(new Draggabilly(e, { containment: '.preview-layout-grid' })));
            }
            else
            {
                console.warn("Draggabilly is not available - drag and drop interface will not work.");
            }

            $('#grid__tmp').remove();

            this.pk.initShiftLayout(Array.from(document.querySelectorAll('.preview-layout-grid > .preview-layout-item')));
        }
    }

    private initUser(): void
    {
        if (window.ccOptions.baseUrl)
        {
            this.conn = new HAConnection(window.ccOptions.baseUrl);
        }

        this.conn.OnConnectionStateChanged.on(state =>
        {
            if (state == HAConnectionState.Closed)
            {
                $('#alerts').show().find('.alert-message').text('[H] Connection lost, reconnecting...');
            }
            else if (state == HAConnectionState.Open)
            {
                $('#alerts').hide();
            }
        });

        this.conn.OnStateChanged.on(state =>
        {
            var tiles = this.findTilesByEntityId(state.data.entity_id);
            for (var t of tiles)
            {
                t.updateState(state.data);
                console.info(`Updating tile for entity "${state.data.entity_id}" to state "${state.data.new_state.state}".`);
            }
        });

        this.conn.initialize();

        this.tileConn = new signalR.HubConnectionBuilder().withUrl('/hubs/tile').build();
        this.tileConn.onclose(e =>
        {
            $('#alerts').show().find('.alert-message').text('[S] Connection lost, reconnecting...');
            window.setTimeout(() =>
            {
                window.location.reload();
            }, 10000);
        });
        this.tileConn.start().then(() =>
        {
            $('.tiles .tile').each((_, e) =>
            {
                try
                {
                    let tile: Tile = new TileMap.ClassMap[$(e).data('tile-type').toString()](window.ccOptions.pageId, $(e).data('tile-name'), this.tileConn, this.conn);
                    this.tiles.push(tile);
                }
                catch (ex)
                {
                    console.error('Error instantiating class "' + ($(e).data('tile-type') || '__MISSING__') + 'Tile". Was it added to the tile type map?', ex, e);
                }
            });

            if (!this.initHandle)
            {
                this.initHandle = window.setInterval(() => this.waitAndPerformInit(), 25)
            }

            if (window.ccOptions.autoReturn > 0)
            {
                window.setTimeout(() => window.location.href = '/d/', window.ccOptions.autoReturn * 1000);
            }
        });
    }

    private waitAndPerformInit()
    {
        if (!this.conn)
        {
            console.warn('Cancelling tile initialization - connection is not set up.');
            window.clearInterval(this.initHandle);
        }

        if (this.conn.ConnectionState !== HAConnectionState.Open)
            return;

        if (this.tiles.filter(t => t.loaded).length !== this.tiles.length)
            return;

        // Now, refresh all of them at once
        this.conn.refreshAllStates();

        window.clearInterval(this.initHandle);
    }

    private findTilesByEntityId(entityId: string): Tile[]
    {
        return this.tiles.filter(t =>
        {
            let definedIds = t.getEntityIds();
            return definedIds.some(e => e.toLowerCase() === entityId.toLowerCase());
        });
    }

    protected initializeNightlyRefresh(): void
    {
        // Workaround to get around SignalR hub being kinda crappy :/
        window.setInterval(() =>
        {
            if (new Date().getHours() == 2)
            {
                window.setTimeout(() =>
                {
                    window.location.reload();
                }, 3600000); // 1 hour
            }
        }, 3500000) // ~58 minutes
    }

    protected initializeMdiPreview(): void
    {
        $('.mdi-icon-placeholder + input').each((_, e) =>
        {
            $(e).keyup((el) =>
            {
                this.refreshDynamicIcon(el.currentTarget);
            });
            this.refreshDynamicIcon(e);
        });
    }

    protected refreshDynamicIcon(target: HTMLElement): void
    {
        $(target).parent().children('.mdi-icon-placeholder').attr('class', 'large icon mdi-icon-placeholder').addClass(`mdi mdi-${$(target).val()}`);
    }

    protected initializeColorPreview(): void
    {
        $('.color-preview + input').each((_, e) =>
        {
            $(e).keyup((el) =>
            {
                this.refreshDynamicColor(el.currentTarget);
            });
            this.refreshDynamicColor(e);
        });
    }

    protected refreshDynamicColor(target: HTMLElement): void
    {
        $(target).parent().children('.color-preview').css('color', `${$(target).val()}`);
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